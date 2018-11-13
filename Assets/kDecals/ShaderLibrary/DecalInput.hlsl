#ifndef DECAL_INPUT_INCLUDED
#define DECAL_INPUT_INCLUDED

// -------------------------------------------------- //
//                      INCLUDES                      //
// -------------------------------------------------- //

#include "UnityCG.cginc"

// -------------------------------------------------- //
//                      UNIFORMS                      //
// -------------------------------------------------- //

// TODO
// - Move to InstancedProperty
int _Axis;

float4x4 unity_Projector;
SamplerState _Linear_Clamp_sampler;

// -------------------------------------------------- //
//                     ATTRIBUTES                     //
// -------------------------------------------------- //

struct AttributesDecal
{
    // Common
    float4 vertex    : POSITION;
    float3 normal    : NORMAL;

    // Lighting
#ifdef _LIGHTING
    half4 texcoord0 : TEXCOORD1;
    half4 texcoord1 : TEXCOORD1;
#if defined(DYNAMICLIGHTMAP_ON) || defined(UNITY_PASS_META)
    half4 texcoord2 : TEXCOORD2;
#endif // defined(DYNAMICLIGHTMAP_ON) || defined(UNITY_PASS_META)
#ifdef _TANGENT_TO_WORLD
    half4 tangent   : TANGENT;
#endif // _TANGENT_TO_WORLD
#endif // _LIGHTING

    // Instancing
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

// -------------------------------------------------- //
//                      VARYINGS                      //
// -------------------------------------------------- //

struct VaryingsDecal
{
    // Common
    float4 positionCS                       : SV_POSITION;
    float3 normalOS                         : NORMAL;
    float4 uv0                              : TEXCOORD0;

    // Lighting
#ifdef _LIGHTING
    float4 tangentOS                        : TANGENT;
    half4 uv1                               : TEXCOORD1;
    float4 positionWS                       : TEXCOORD3;
    float4 viewDir                          : TEXCOORD4;
    float4 tangentToWorld[3]                : TEXCOORD5;
    half4 ambientOrLightmapUV              : TEXCOORD8;
#endif

    // Fog
#ifdef _FOG
#ifdef _LIGHTING
    UNITY_FOG_COORDS(9)
#else
    UNITY_FOG_COORDS(1)
#endif
#endif
};

// -------------------------------------------------- //
//                     DECAL DATA                     //
// -------------------------------------------------- //

struct DecalData
{
    // Common
    float3 normalOS : TEXCOORD10;
    float4 uv0      : TEXCOORD11;
};

DecalData PackVaryingsToDecalData(VaryingsDecal IN)
{
    // Initialize
    DecalData o;
    UNITY_INITIALIZE_OUTPUT(DecalData, o);

    // Copy data
    o.normalOS = IN.normalOS;
    o.uv0 = IN.uv0;

    // Finalize
    return o;
}

// -------------------------------------------------- //
//                      SAMPLING                      //
// -------------------------------------------------- //

float4 SampleDecal(DecalData decalData, Texture2D decalTexture, float4 defaultColor)
{
    // Sample Decal texture
    half4 tex = decalTexture.Sample(_Linear_Clamp_sampler, decalData.uv0.xy / decalData.uv0.w);

    // Clamp to projection
    half4 col;
    if(_Axis == 2 || _Axis == 3) // Y
        col = lerp(defaultColor, tex, (1 - saturate(decalData.uv0.z)) * abs(decalData.normalOS.y));
    else if(_Axis == 4 || _Axis == 5) // Z
        col = lerp(defaultColor, tex, (1 - saturate(decalData.uv0.z)) * abs(decalData.normalOS.z));
    else // X
        col = lerp(defaultColor, tex, (1 - saturate(decalData.uv0.z)) * abs(decalData.normalOS.x));

    // Finalize
    return col;
}

#endif
