#ifndef UNIVERSAL_LIT_INPUT_INCLUDED
#define UNIVERSAL_LIT_INPUT_INCLUDED

// -------------------------------------
// Includes
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"
#include "Packages/com.kink3d.decals/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

// -------------------------------------
// Uniforms
CBUFFER_START(UnityPerMaterial)
float4 _BaseMap_ST;
half4 _BaseColor;
half4 _SpecColor;
half4 _EmissionColor;
half _Cutoff;
half _Smoothness;
half _Metallic;
half _BumpScale;
half _OcclusionStrength;
CBUFFER_END

TEXTURE2D(_BaseMap);
TEXTURE2D(_BumpMap);
TEXTURE2D(_EmissionMap);
TEXTURE2D(_OcclusionMap);
TEXTURE2D(_MetallicGlossMap);
TEXTURE2D(_SpecGlossMap);

// -------------------------------------
// Macros
#ifdef _SPECULAR_SETUP
    #define SAMPLE_METALLICSPECULAR(positionPS) SAMPLE_DECAL2D(_SpecGlossMap, positionPS);
#else
    #define SAMPLE_METALLICSPECULAR(positionPS) SAMPLE_DECAL2D(_MetallicGlossMap, positionPS);
#endif

// -------------------------------------
// Material Helpers
half Alpha(half albedoAlpha, half4 color, half cutoff)
{
#if !defined(_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A) && !defined(_GLOSSINESS_FROM_BASE_ALPHA)
    half alpha = albedoAlpha * color.a;
#else
    half alpha = color.a;
#endif

#if defined(_ALPHATEST_ON)
    clip(alpha - cutoff);
#endif

    return alpha;
}

half4 SampleAlbedoAlpha(float2 uv, TEXTURE2D_PARAM(albedoAlphaMap, sampler_albedoAlphaMap))
{
    return SAMPLE_TEXTURE2D(albedoAlphaMap, sampler_albedoAlphaMap, uv);
}

half4 SampleMetallicSpecGloss(float4 positionPS, half albedoAlpha)
{
    half4 specGloss;

#ifdef _METALLICSPECGLOSSMAP
    specGloss = SAMPLE_METALLICSPECULAR(positionPS);
    #ifdef _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
        specGloss.a = albedoAlpha * _Smoothness;
    #else
        specGloss.a *= _Smoothness;
    #endif
#else // _METALLICSPECGLOSSMAP
    #if _SPECULAR_SETUP
        specGloss.rgb = _SpecColor.rgb;
    #else
        specGloss.rgb = _Metallic.rrr;
    #endif

    #ifdef _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
        specGloss.a = albedoAlpha * _Smoothness;
    #else
        specGloss.a = _Smoothness;
    #endif
#endif

    return specGloss;
}

half SampleOcclusion(float4 positionPS)
{
#ifdef _OCCLUSIONMAP
// TODO: Controls things like these by exposing SHADER_QUALITY levels (low, medium, high)
#if defined(SHADER_API_GLES)
    return SAMPLE_DECAL2D(_OcclusionMap, positionPS);
#else
    half occ = SAMPLE_DECAL2D(_OcclusionMap, positionPS);
    return LerpWhiteTo(occ, _OcclusionStrength);
#endif
#else
    return 1.0;
#endif
}

half3 SampleNormal(float4 positionPS)
{
#ifdef _NORMALMAP
    half4 normalMap = SAMPLE_DECAL2D(_BumpMap, positionPS);
    #if BUMP_SCALE_NOT_SUPPORTED
        return UnpackNormal(normalMap);
    #else
        return UnpackNormalScale(normalMap, _BumpScale);
    #endif
#else
    return half3(0.0h, 0.0h, 1.0h);
#endif
}

half3 SampleEmission(float4 positionPS)
{
#ifndef _EMISSION
    return 0;
#else
    half4 emissionMap = SAMPLE_DECAL2D(_EmissionMap, positionPS);
    return emissionMap.rgb * _EmissionColor;
#endif
}

inline void InitializeStandardLitSurfaceData(float4 positionPS, out SurfaceData outSurfaceData)
{
    half4 albedoAlpha = SAMPLE_DECAL2D(_BaseMap, positionPS);
    outSurfaceData.alpha = Alpha(albedoAlpha.a, _BaseColor, _Cutoff);

    half4 specGloss = SampleMetallicSpecGloss(positionPS, albedoAlpha.a);
    outSurfaceData.albedo = albedoAlpha.rgb * _BaseColor.rgb;

#if _SPECULAR_SETUP
    outSurfaceData.metallic = 1.0h;
    outSurfaceData.specular = specGloss.rgb;
#else
    outSurfaceData.metallic = specGloss.r;
    outSurfaceData.specular = half3(0.0h, 0.0h, 0.0h);
#endif

    outSurfaceData.smoothness = specGloss.a;
    outSurfaceData.normalTS = SampleNormal(positionPS);
    outSurfaceData.occlusion = SampleOcclusion(positionPS);
    outSurfaceData.emission = SampleEmission(positionPS);

    // TODO: Support ClearCoat
    outSurfaceData.clearCoatMask = 0;
    outSurfaceData.clearCoatSmoothness = 0;
}

#endif
