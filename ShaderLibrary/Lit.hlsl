#ifndef DECAL_LIT_INCLUDED
#define DECAL_LIT_INCLUDED

// Includes
#include "UnityStandardCore.cginc"
#include "Packages/com.kink3d.decals/ShaderLibrary/Core.hlsl"

// Attributes
struct Attributes
{
    float4 positionOS           : POSITION;
    float3 normalOS             : NORMAL;
    half4 texcoord0             : TEXCOORD1;
    half4 texcoord1             : TEXCOORD1;
#if defined(DYNAMICLIGHTMAP_ON) || defined(UNITY_PASS_META)
    half4 texcoord2             : TEXCOORD2;
#endif
#ifdef _TANGENT_TO_WORLD
    half4 tangentOS             : TANGENT;
#endif
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

// Varyings
struct Varyings
{
    float4 positionCS           : SV_POSITION;
    float4 positionPS           : TEXCOORD0;
    float3 normalWS             : NORMAL;
    float4 tangentOS            : TANGENT;
    float4 uv1                  : TEXCOORD1;
    float4 positionWS           : TEXCOORD3;
    float4 viewDirectionWS      : TEXCOORD4;
    float4 tangentToWorld[3]    : TEXCOORD5;
    half4 ambientOrLightmapUV   : TEXCOORD8;
    UNITY_FOG_COORDS(9)
};

// Uniforms
Texture2D _BaseTex;
Texture2D _SpecGlossTex;
Texture2D _MetallicGlossTex;
Texture2D _BumpTex;
Texture2D _OcclusionTex;
Texture2D _EmissionTex;

// Normal
float3 Normal(float3 normalTS, float4 tangentToWorld[3])
{
#ifdef _NORMALMAP
    // Split TangentToWorld
    half3 tangent = tangentToWorld[0].xyz;
    half3 binormal = tangentToWorld[1].xyz;
    half3 normal = tangentToWorld[2].xyz;

    // Orthonormalize
    #if UNITY_TANGENT_ORTHONORMALIZE
        normal = NormalizePerPixelNormal(normal);
        tangent = normalize (tangent - normal * dot(tangent, normal));
        half3 newB = cross(normal, tangent);
        binormal = newB * sign (dot (newB, binormal));
    #endif
    
    // Finalize
    return NormalizePerPixelNormal(tangent * normalTS.x + binormal * normalTS.y + normal * normalTS.z);
#else
    return normalize(tangentToWorld[2].xyz);
#endif
}

// MetallicSpecGloss
half4 SampleMetallicSpecGloss(float4 uv)
{
    half4 specGloss;
    #ifdef _METALLICSPECGLOSSMAP
        #ifdef _SPECULAR_SETUP
            specGloss = SampleDecalTexture(_SpecGlossTex, uv);
        #else
            specGloss = SampleDecalTexture(_MetallicGlossTex, uv);
        #endif
        specGloss.a *= _Glossiness;
    #else
        #if _SPECULAR_SETUP
            specGloss.rgb = _SpecColor.rgb;
        #else
            specGloss.rgb = _Metallic.rrr;
        #endif
        specGloss.a = _Glossiness;
    #endif
    return specGloss;
}

// Packing
VertexInput PackAttributesToVertexInput(Attributes input)
{
    // Initialize
    VertexInput output;
    UNITY_INITIALIZE_OUTPUT(VertexInput, output);

    // Copy data
    output.vertex = input.positionOS;
    output.normal = input.normalOS;
    output.uv0 = input.texcoord0;
    output.uv1 = input.texcoord1;
#if defined(DYNAMICLIGHTMAP_ON) || defined(UNITY_PASS_META)
    output.uv2 = input.texcoord2;
#endif
#ifdef _TANGENT_TO_WORLD
    output.tangent = input.tangentOS;
#endif

    return output;
}

// FragmentCommonData
inline FragmentCommonData BuildFragmentCommonData (half3 color, half3 specularColor, half smoothness, half alpha, 
    float3 positionWS, float3 normalWS, float3 viewDirectionWS, float4 tangentToWorld[3])
{
    FragmentCommonData output = (FragmentCommonData)0;
    half oneMinusReflectivity;
    half3 specColor;
    #ifdef _SPECULAR_SETUP
        half3 diffColor = EnergyConservationBetweenDiffuseAndSpecular (color, specularColor, /*out*/ oneMinusReflectivity);
        specColor = specularColor;
    #else
        half3 diffColor = DiffuseAndSpecularFromMetallic(color, specularColor.r, /*out*/ specColor, /*out*/ oneMinusReflectivity);
    #endif

    output.diffColor = diffColor;
    output.normalWorld = Normal(normalWS, tangentToWorld);
    output.specColor = specColor;
    output.smoothness = smoothness;
    output.alpha = alpha;
    output.oneMinusReflectivity = oneMinusReflectivity;
    output.eyeVec = NormalizePerPixelNormal(-viewDirectionWS);
    output.posWorld = positionWS;
    return output;
}

// Vertex
Varyings Vertex (Attributes input)
{
    Varyings output;
    UNITY_INITIALIZE_OUTPUT(Varyings, output);
    
    output.positionCS = UnityObjectToClipPos(input.positionOS);
    output.positionPS = UnityObjectToProjectionPos(input.positionOS);
    output.normalWS = mul(UNITY_MATRIX_M, input.normalOS);

    #ifdef _TANGENT_TO_WORLD
    output.tangentOS = input.tangentOS;
#endif
    output.uv1 = input.texcoord0;
    output.positionWS = mul(unity_ObjectToWorld, input.positionOS);
    output.viewDirectionWS = float4(NormalizePerVertexNormal(output.positionWS.xyz - _WorldSpaceCameraPos), 1);

    // TangentToWorld
#ifdef _TANGENT_TO_WORLD
    float4 tangentWS = float4(UnityObjectToWorldDir(output.tangentOS.xyz), output.tangentOS.w);
    float3x3 tangentToWorld = CreateTangentToWorldPerVertex(output.normalWS, tangentWS.xyz, tangentWS.w);
    output.tangentToWorld[0].xyz = tangentToWorld[0];
    output.tangentToWorld[1].xyz = tangentToWorld[1];
    output.tangentToWorld[2].xyz = tangentToWorld[2];
#else
    output.tangentToWorld[0].xyz = 0;
    output.tangentToWorld[1].xyz = 0;
    output.tangentToWorld[2].xyz = output.normalWS;
#endif

    // Shadow Receiving
    UNITY_TRANSFER_LIGHTING(output, input.uv1);

    // Fog
    UNITY_TRANSFER_FOG(output, output.positionCS);
    
    // Pack to VertexInput to Lightmap/SH coords
    VertexInput vertexInput = PackAttributesToVertexInput(input);
    output.ambientOrLightmapUV = VertexGIForward(vertexInput, output.positionWS, output.normalWS);

    return output;
}

// Fragment
half4 FragmentLit (Varyings input, half3 color, half3 specularColor, half smoothness, half3 normal, 
    half3 emission, half occlusion, half alpha, half cutoff)
{
    // Setup
    UNITY_APPLY_DITHER_CROSSFADE(input.positionCS.xy);
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    // Common data
    FragmentCommonData commonData = BuildFragmentCommonData(color, specularColor, smoothness, alpha, 
        input.positionWS, normal, input.viewDirectionWS, input.tangentToWorld);

    UnityLight mainLight = MainLight ();
    UNITY_LIGHT_ATTENUATION(atten, input, input.positionWS);
    
    // Lighting
    UnityGI gi = FragmentGI (commonData, occlusion, input.ambientOrLightmapUV, atten, mainLight);
    half4 finalColor = UNITY_BRDF_PBS (commonData.diffColor, commonData.specColor, commonData.oneMinusReflectivity, 
        commonData.smoothness, commonData.normalWorld, commonData.eyeVec, gi.light, gi.indirect);

    // Alpha
    color = AlphaPremultiply(finalColor, alpha);
    AlphaClip(alpha, cutoff);
        
    finalColor.rgb += emission;
    finalColor.a = alpha;
    finalColor = ClampProjection(finalColor, input.positionPS);
    UNITY_APPLY_FOG(input.fogCoord, finalColor.rgb);
    return finalColor;
}

#endif
