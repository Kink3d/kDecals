#ifndef DECAL_CORE_INCLUDED
#define DECAL_CORE_INCLUDED

// -------------------------------------
// Uniforms
float4x4 decal_Projection;
float3 decal_Direction;
half decal_DepthFalloff;
half decal_Angle;
half decal_AngleFalloff;
float decal_DeferredFlags;

SAMPLER(_Linear_Clamp_sampler);

// -------------------------------------
// Macros
#define SAMPLE_DECAL2D(texture, positionPS) SAMPLE_TEXTURE2D(texture, _Linear_Clamp_sampler, positionPS.xy / positionPS.w);

// -------------------------------------
// Projection
float4 TransformObjectToProjection(float4 positionOS)
{
    return mul(decal_Projection, mul(UNITY_MATRIX_M, positionOS));
}

bool ClampProjection(float4 positionPS, float3 normalWS)
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
    
    // Finalize
    half value = boundsClamp * zClamp * angleClamp * blend;
    return value <= 0.001;
}

#endif
