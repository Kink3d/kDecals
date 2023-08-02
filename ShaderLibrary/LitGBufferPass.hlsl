#ifndef DECAL_LIT_GBUFFER_PASS_INCLUDED
#define DECAL_LIT_GBUFFER_PASS_INCLUDED

// -------------------------------------
// Includes
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityGBuffer.hlsl"

// -------------------------------------
// GBuffer Uniforms

// -------------------------------------
// Structs
struct Attributes
{
    float4 positionOS   : POSITION;
    float3 normalOS     : NORMAL;
    float4 tangentOS    : TANGENT;
    float2 texcoord     : TEXCOORD0;
    float2 lightmapUV   : TEXCOORD1;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float4 positionPS               : TEXCOORD0;
    DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 1);

#ifdef _ADDITIONAL_LIGHTS
    float3 positionWS               : TEXCOORD2;
#endif

#ifdef _NORMALMAP
    float4 normalWS                 : TEXCOORD3;    // xyz: normal, w: viewDir.x
    float4 tangentWS                : TEXCOORD4;    // xyz: tangent, w: viewDir.y
    float4 bitangentWS              : TEXCOORD5;    // xyz: bitangent, w: viewDir.z
#else
    float3 normalWS                 : TEXCOORD3;
    float3 viewDirWS                : TEXCOORD4;
#endif

    half4 fogFactorAndVertexLight   : TEXCOORD6; // x: fogFactor, yzw: vertex light

#ifdef _MAIN_LIGHT_SHADOWS
    float4 shadowCoord              : TEXCOORD7;
#endif

    float4 positionCS               : SV_POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

// -------------------------------------
// InputData
void InitializeInputData(Varyings input, half3 normalTS, out InputData inputData)
{
    inputData = (InputData)0;

#ifdef _ADDITIONAL_LIGHTS
    inputData.positionWS = input.positionWS;
#endif

#ifdef _NORMALMAP
    half3 viewDirWS = half3(input.normalWS.w, input.tangentWS.w, input.bitangentWS.w);
    inputData.normalWS = TransformTangentToWorld(normalTS,
        half3x3(input.tangentWS.xyz, input.bitangentWS.xyz, input.normalWS.xyz));
#else
    half3 viewDirWS = input.viewDirWS;
    inputData.normalWS = input.normalWS;
#endif

    inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
    viewDirWS = SafeNormalize(viewDirWS);

    inputData.viewDirectionWS = viewDirWS;
#if defined(_MAIN_LIGHT_SHADOWS) && !defined(_RECEIVE_SHADOWS_OFF)
    inputData.shadowCoord = input.shadowCoord;
#else
    inputData.shadowCoord = float4(0, 0, 0, 0);
#endif
    inputData.fogCoord = input.fogFactorAndVertexLight.x;
    inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
    inputData.bakedGI = SAMPLE_GI(input.lightmapUV, input.vertexSH, inputData.normalWS);
}

// -------------------------------------
// Vertex
Varyings LitGBufferPassVertex(Attributes input)
{
    Varyings output = (Varyings)0;

    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
    half3 viewDirWS = GetCameraPositionWS() - vertexInput.positionWS;
    half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);
    half fogFactor = ComputeFogFactor(vertexInput.positionCS.z);

    output.positionPS = TransformObjectToProjection(input.positionOS);

#ifdef _NORMALMAP
    output.normalWS = half4(normalInput.normalWS, viewDirWS.x);
    output.tangentWS = half4(normalInput.tangentWS, viewDirWS.y);
    output.bitangentWS = half4(normalInput.bitangentWS, viewDirWS.z);
#else
    output.normalWS = NormalizeNormalPerVertex(normalInput.normalWS);
    output.viewDirWS = viewDirWS;
#endif

    OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, output.lightmapUV);
    OUTPUT_SH(output.normalWS.xyz, output.vertexSH);

    output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);

#ifdef _ADDITIONAL_LIGHTS
    output.positionWS = vertexInput.positionWS;
#endif

#if defined(_MAIN_LIGHT_SHADOWS) && !defined(_RECEIVE_SHADOWS_OFF)
    output.shadowCoord = GetShadowCoord(vertexInput);
#endif

    output.positionCS = vertexInput.positionCS;

    return output;
}

// -------------------------------------
// Fragment

FragmentOutput EncodeSurfaceDataToGbuffer(SurfaceData surfaceData, InputData inputData, half3 globalIllumination, int lightingMode, float4 positionCS)
{
    uint materialFlags = 0;

    #ifdef _RECEIVE_SHADOWS_OFF
    materialFlags |= kMaterialFlagReceiveShadowsOff;
    #endif

    #if defined(LIGHTMAP_ON) && defined(_MIXED_LIGHTING_SUBTRACTIVE)
    materialFlags |= kMaterialFlagSubtractiveMixedLighting;
    #endif
    
    half3 packedNormalWS = PackNormal(inputData.normalWS);
    float2 positionSS = GetNormalizedScreenSpaceUV(positionCS);
    
    // Calculate colors
    half3 albedo = surfaceData.albedo.rgb;
    half3 specular = surfaceData.specular.rgb;
    half smoothness = surfaceData.smoothness;
    half3 normal = packedNormalWS;
    half occlusion = surfaceData.occlusion;
    half mFlags = PackMaterialFlags(materialFlags);

    // Write to GBuffer
    FragmentOutput output;
    output.GBuffer0 = half4(albedo, mFlags);            // albedo          albedo          albedo          materialFlags   (sRGB rendertarget)
    output.GBuffer1 = half4(specular, occlusion);       // specular        specular        specular        occlusion
    output.GBuffer2 = half4(normal, smoothness);        // encoded-normal  encoded-normal  encoded-normal  smoothness                 
    // output.GBuffer3 = half4(globalIllumination, 1);  // GI              GI              GI              [optional: see OutputAlpha()] (lighting buffer)

    #if _RENDER_PASS_ENABLED
    output.GBuffer4 = inputData.positionCS.z;
    #endif

    #if OUTPUT_SHADOWMASK
    output.GBUFFER_SHADOWMASK = inputData.shadowMask; // will have unity_ProbesOcclusion value if subtractive lighting is used (baked)
    #endif

    #ifdef _LIGHT_LAYERS
    uint renderingLayers = GetMeshRenderingLightLayer();
    // Note: we need to mask out only 8bits of the layer mask before encoding it as otherwise any value > 255 will map to all layers active
    output.GBUFFER_LIGHT_LAYERS = float4((renderingLayers & 0x000000FF) / 255.0, 0.0, 0.0, 0.0);
    #endif

    return output;
}

FragmentOutput LitGBufferPassFragment(Varyings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    SurfaceData surfaceData;
    InitializeStandardLitSurfaceData(input.positionPS, surfaceData);

    InputData inputData;
    InitializeInputData(input, surfaceData.normalTS, inputData);

    if(ClampProjection(input.positionPS, inputData.normalWS))
        discard;

    // in LitForwardPass GlobalIllumination (and temporarily LightingPhysicallyBased) are called inside UniversalFragmentPBR
    // in Deferred rendering we store the sum of these values (and of emission as well) in the GBuffer
    BRDFData brdfData;
    InitializeBRDFData(surfaceData.albedo, surfaceData.metallic, surfaceData.specular, surfaceData.smoothness, surfaceData.alpha, brdfData);

    Light mainLight = GetMainLight(inputData.shadowCoord, inputData.positionWS, inputData.shadowMask);
    MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI, inputData.shadowMask);
    half3 color = GlobalIllumination(brdfData, inputData.bakedGI, surfaceData.occlusion, inputData.positionWS, inputData.normalWS, inputData.viewDirectionWS);

    return EncodeSurfaceDataToGbuffer(surfaceData, inputData, surfaceData.emission + color, 0, input.positionCS);
}

#endif