#ifndef DECAL_CORE_INCLUDED
#define DECAL_CORE_INCLUDED

// -------------------------------------
// Uniforms
float4x4 decal_Projection;
float3 decal_Direction;
half decal_DepthFalloff;
half decal_Angle;
half decal_AngleFalloff;

half4 _ZeroColor;

SAMPLER(_Linear_Clamp_sampler);

// -------------------------------------
// Macros
#define SAMPLE_DECAL2D(texture, positionPS) SAMPLE_TEXTURE2D(texture, _Linear_Clamp_sampler, positionPS.xy / positionPS.w);
#define CLAMP_PROJECTION(color, positionPS, normalWS) color = ClampProjection(color, positionPS, normalWS);

// -------------------------------------
// Projection
float4 TransformObjectToProjection(float4 positionOS)
{
    return mul(decal_Projection, mul(UNITY_MATRIX_M, positionOS));
}

half4 ClampProjection(half4 color, float4 positionPS, float3 normalWS)
{
    // Clamp outside bounds (positionPS.x > 1 || positionPS.x < 0 || positionPS.y > 1 || positionPS.y < 0)
    float2 d = abs(positionPS.xy * 2 - 1) - 1;
    d = 1 - d / fwidth(d);
    half boundsClamp = saturate(min(d.x, d.y));
    
    // Clamp behind Decal (VectorPosition < ClampPosition)
    float3 projectionVectorPosition = (positionPS * normalize(mul(decal_Projection, float4(-decal_Direction, 0)))).xyz;
    half clampPosition = 1.001;
    half zClamp = 1.0 - step(clampPosition, projectionVectorPosition.z); 

    // Clamp angle (Angle < decal_Angle)
    half angleR = acos(dot(normalize(normalWS), normalize(decal_Direction)));
    half angleClamp = 1.0 - step(angleR, decal_Angle);

    // Blending
    half depthBlend = smoothstep(0, decal_DepthFalloff, positionPS.z);
    half rangedAngle = (angleR - decal_Angle) / (3.14 - decal_Angle);
    half angleBlend = smoothstep(0, decal_AngleFalloff, rangedAngle);
    half blend = depthBlend * angleBlend;

    // return pow(angleBlend, 2.2);

    // BlendZeroColor
    half4 blendZeroColor = _ZeroColor;
    #if defined(_BLEND_ALPHA)
        blendZeroColor = half4(color.rgb, 0);
    #endif
    
    // Finalize
    half blendFactor = boundsClamp * zClamp * angleClamp * blend;
    return lerp(blendZeroColor, color, saturate(blendFactor));
}

#endif
