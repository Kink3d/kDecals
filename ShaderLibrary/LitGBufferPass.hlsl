#ifndef DECAL_LIT_GBUFFER_PASS_INCLUDED
#define DECAL_LIT_GBUFFER_PASS_INCLUDED

// -------------------------------------
// Includes
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityGBuffer.hlsl"

// -------------------------------------
// GBuffer Uniforms

#ifdef _DECAL_PER_CHANNEL
TEXTURE2D(_GBuffer0Copy);
TEXTURE2D(_GBuffer1Copy);
TEXTURE2D(_GBuffer2Copy);
TEXTURE2D(_GBuffer3Copy);

SAMPLER(sampler_GBuffer0Copy);
SAMPLER(sampler_GBuffer1Copy);
SAMPLER(sampler_GBuffer2Copy);
SAMPLER(sampler_GBuffer3Copy);
#endif

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

#ifdef _DECALTYPE_PROJECTION
    output.positionPS = TransformObjectToProjection(input.positionOS);
#else
    output.uv = input.texcoord;
#endif

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

    #ifdef _DECAL_PER_CHANNEL
    // Unpack Flags
    int flags = decal_DeferredFlags;
    bool affectAlbedo = fmod(flags, 2) == 1;
    bool affectSpecular = fmod(flags, 4) >= 2;
    bool affectSmoothness = fmod(flags, 8) >= 4;
    bool affectNormal = fmod(flags, 16) >= 8;
    bool affectOcclusion = fmod(flags, 32) >= 16;

    // Sample GBuffer Copies
    half4 gbuffer0 = SAMPLE_TEXTURE2D(_GBuffer0Copy, sampler_GBuffer0Copy, positionSS);
    half4 gbuffer1 = SAMPLE_TEXTURE2D(_GBuffer1Copy, sampler_GBuffer1Copy, positionSS);
    half4 gbuffer2 = SAMPLE_TEXTURE2D(_GBuffer2Copy, sampler_GBuffer2Copy, positionSS);
    half4 gbuffer3 = SAMPLE_TEXTURE2D(_GBuffer3Copy, sampler_GBuffer3Copy, positionSS);
    
    // Disable Channels
    albedo = lerp(gbuffer0.rgb, albedo, (affectAlbedo ? 1 : 0));
    specular = lerp(gbuffer1.rgb, specular, (affectSpecular ? 1 : 0));
    smoothness = lerp(gbuffer2.a, smoothness, (affectSmoothness ? 1 : 0));
    normal = lerp(gbuffer2.rgb, normal, (affectNormal ? 1 : 0));
    occlusion = lerp(gbuffer1.a, occlusion, (affectOcclusion ? 1 : 0));
    #endif

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

#ifdef _DECALTYPE_PROJECTION
    float4 uv = input.positionPS;
#else
    float4 uv = input.uv;
#endif

    SurfaceData surfaceData;
    InitializeStandardLitSurfaceData(uv, surfaceData);

    InputData inputData;
    InitializeInputData(input, surfaceData.normalTS, inputData);

#ifdef _DECALTYPE_PROJECTION
    if(ClampProjection(input.positionPS, inputData.normalWS))
        discard;
#endif

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