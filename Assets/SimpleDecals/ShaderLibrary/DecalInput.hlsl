#ifndef DECAL_INPUT_INCLUDED
#define DECAL_INPUT_INCLUDED

float4x4 unity_Projector;
Texture2D _DecalTex;
SamplerState _Linear_Clamp_sampler;

// TODO - Move to Instanced
int _Axis;

struct AttributesDecal
{
    float4 vertex : POSITION;
    float3 normal : NORMAL;
};

struct VaryingsDecal 
{
    float4 positionCS : SV_POSITION;
    float3 normalOS : NORMAL;
    float4 uv0 : TEXCOORD0;
    //UNITY_FOG_COORDS(2)
};

VaryingsDecal VertexDecal (AttributesDecal v)
{
    VaryingsDecal o;
    o.positionCS = UnityObjectToClipPos(v.vertex);
    o.normalOS = v.normal;
    o.uv0 = mul (unity_Projector, v.vertex);
    //UNITY_TRANSFER_FOG(o,o.positionCS);
    return o;
}

float4 SampleDecal(VaryingsDecal IN, float4 defaultColor)
{
    fixed4 tex = _DecalTex.Sample(_Linear_Clamp_sampler, IN.uv0.xy / IN.uv0.w);
    if(_Axis == 2 || _Axis == 3)
        return lerp(defaultColor, tex, saturate(sign(IN.uv0.y) * abs(IN.normalOS.y)));
    else if(_Axis == 4 || _Axis == 5)
        return lerp(defaultColor, tex, saturate(sign(IN.uv0.z) * abs(IN.normalOS.z)));
    else
        return lerp(defaultColor, tex, saturate(sign(IN.uv0.x) * abs(IN.normalOS.x)));
}

#endif
