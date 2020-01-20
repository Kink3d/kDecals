#ifndef DECAL_INPUT_INCLUDED
#define DECAL_INPUT_INCLUDED

// Includes
#include "UnityCG.cginc"

// Uniforms
float4x4 unity_Projector;
SamplerState _Linear_Clamp_sampler;

// Sampling
half4 SampleDecalTexture(Texture2D decalTexture, float4 positionPS)
{
    return decalTexture.Sample(_Linear_Clamp_sampler, positionPS.xy / positionPS.w);
}

// Projection
float4 UnityObjectToProjectionPos(float4 positionOS)
{
    return mul(unity_Projector, positionOS);
}

half4 ClampProjection(half4 color, float4 positionPS)
{
    return lerp(0, color, saturate(positionPS.z));
}

// Alpha
void AlphaClip(half alpha, half cutoff)
{
    #if defined(_ALPHATEST_ON)
        clip(alpha - cutoff);
    #endif
}

half3 AlphaPremultiply(half3 color, half alpha)
{
    #ifdef _ALPHAPREMULTIPLY_ON
        color *= alpha;
    #endif
    return color;
}

#endif
