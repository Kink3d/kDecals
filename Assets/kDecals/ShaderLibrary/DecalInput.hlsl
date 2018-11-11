#ifndef DECAL_INPUT_INCLUDED
#define DECAL_INPUT_INCLUDED

// -------------------------------------------------- //
//                      INCLUDES                      //
// -------------------------------------------------- //

#include "UnityCG.cginc"

// -------------------------------------------------- //
//                      UNIFORMS                      //
// -------------------------------------------------- //

float4x4 unity_Projector;
Texture2D _DecalTex;
SamplerState _Linear_Clamp_sampler;

// TODO
// - Move to InstancedProperty
int _Axis;

// -------------------------------------------------- //
//                     ATTRIBUTES                     //
// -------------------------------------------------- //

struct AttributesDecal
{
    float4 vertex : POSITION;
    float3 normal : NORMAL;
};

// -------------------------------------------------- //
//                      VARYINGS                      //
// -------------------------------------------------- //

struct VaryingsDecal 
{
    float4 positionCS : SV_POSITION;
    float3 normalOS : NORMAL;
    float4 uv0 : TEXCOORD0;
#ifdef _APPLYFOG
    UNITY_FOG_COORDS(2)
#endif
};

// -------------------------------------------------- //
//                       VERTEX                       //
// -------------------------------------------------- //

VaryingsDecal VertexDecal (AttributesDecal v)
{
    VaryingsDecal o;
    o.positionCS = UnityObjectToClipPos(v.vertex);
    o.normalOS = v.normal;
    o.uv0 = mul (unity_Projector, v.vertex);
#ifdef _APPLYFOG
    UNITY_TRANSFER_FOG(o,o.positionCS);
#endif
    return o;
}

// -------------------------------------------------- //
//                      SAMPLING                      //
// -------------------------------------------------- //

float4 SampleDecal(VaryingsDecal IN, float4 defaultColor)
{
    fixed4 tex = _DecalTex.Sample(_Linear_Clamp_sampler, IN.uv0.xy / IN.uv0.w);
    float4 col;
    if(_Axis == 2 || _Axis == 3) // Y
        col = lerp(defaultColor, tex, (1 - saturate(IN.uv0.z)) * abs(IN.normalOS.y));
    else if(_Axis == 4 || _Axis == 5) // Z
        col = lerp(defaultColor, tex, (1 - saturate(IN.uv0.z)) * abs(IN.normalOS.z));
    else // X
        col = lerp(defaultColor, tex, (1 - saturate(IN.uv0.z)) * abs(IN.normalOS.x));
    return col;
}

#endif
