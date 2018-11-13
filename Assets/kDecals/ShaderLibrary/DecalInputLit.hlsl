#ifndef DECAL_INPUT_LIT_INCLUDED
#define DECAL_INPUT_LIT_INCLUDED

// -------------------------------------------------- //
//                      INCLUDES                      //
// -------------------------------------------------- //

#include "UnityStandardCore.cginc"
#include "UnityCG.cginc"

// -------------------------------------------------- //
//                      UNIFORMS                      //
// -------------------------------------------------- //

// Decal
float4x4 unity_Projector;
SamplerState _Linear_Clamp_sampler;

// TODO
// - Move to InstancedProperty
int _Axis;

// Surface
Texture2D _AlbedoTex;
Texture2D _NormalTex;
Texture2D _SpecularTex;
Texture2D _EmissionTex;
float _NormalScale;
float4 _Specular;
float _Smoothness;

// -------------------------------------------------- //
//                     ATTRIBUTES                     //
// -------------------------------------------------- //

struct AttributesDecal
{
    float4 vertex    : POSITION;
    float3 normal    : NORMAL;
    float4 texcoord0 : TEXCOORD1;
    float4 texcoord1 : TEXCOORD1;
#if defined(DYNAMICLIGHTMAP_ON) || defined(UNITY_PASS_META)
    float2 texcoord2 : TEXCOORD2;
#endif
#ifdef _TANGENT_TO_WORLD
    half4 tangent   : TANGENT;
#endif
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

// -------------------------------------------------- //
//                      VARYINGS                      //
// -------------------------------------------------- //

struct VaryingsDecal
{
    float4 positionCS;// : SV_POSITION;
    float3 normalOS;// : NORMAL;
    float4 tangentOS;// : TANGENT;
    float4 uv0;// : TEXCOORD0;
    float4 uv1;// : TEXCOORD1;
    float4 positionWS;// : TEXCOORD3;
#ifdef _APPLYFOG
    UNITY_FOG_COORDS(2)
#endif
};

struct DecalData
{
    float3 normalOS : TEXCOORD10;
    float4 uv0      : TEXCOORD11;
};

// -------------------------------------------------- //
//                      PACKING                       //
// -------------------------------------------------- //

VertexOutputForwardBase PackVaryingsToLegacy(VaryingsDecal IN)
{
    // Initialize
    VertexOutputForwardBase o;
    UNITY_INITIALIZE_OUTPUT(VertexOutputForwardBase, o);

    // Position
    #if UNITY_REQUIRE_FRAG_WORLDPOS
        #if UNITY_PACK_WORLDPOS_WITH_TANGENT
            o.tangentToWorldAndPackedData[0].w = IN.positionWS.x;
            o.tangentToWorldAndPackedData[1].w = IN.positionWS.y;
            o.tangentToWorldAndPackedData[2].w = IN.positionWS.z;
        #else
            o.posWorld = IN.positionWS.xyz;
        #endif
    #endif
    o.pos = IN.positionCS;

    // Texture Coords
    o.tex = IN.uv0;

    // View Vector
    o.eyeVec = float4(NormalizePerVertexNormal(IN.positionWS.xyz - _WorldSpaceCameraPos), 1);

    // TangentToWorld
    #ifdef _TANGENT_TO_WORLD
        float4 tangentWorld = float4(UnityObjectToWorldDir(IN.tangentOS.xyz), IN.tangentOS.w);

        float3x3 tangentToWorld = CreateTangentToWorldPerVertex(IN.normalOS, tangentWorld.xyz, tangentWorld.w);
        o.tangentToWorldAndPackedData[0].xyz = tangentToWorld[0];
        o.tangentToWorldAndPackedData[1].xyz = tangentToWorld[1];
        o.tangentToWorldAndPackedData[2].xyz = tangentToWorld[2];
    #else
        o.tangentToWorldAndPackedData[0].xyz = 0;
        o.tangentToWorldAndPackedData[1].xyz = 0;
        o.tangentToWorldAndPackedData[2].xyz = IN.normalOS;
    #endif

    // Shadow Receiving
    UNITY_TRANSFER_LIGHTING(o, IN.uv1);

    // Finalize
    UNITY_TRANSFER_FOG(o,o.pos);
    return o;
}

VertexInput PackAttributesToVertexInput(AttributesDecal IN)
{
    VertexInput o;
    o.vertex = IN.vertex;
    o.normal = IN.normal;
    o.uv0 = IN.texcoord0;
    o.uv1 = IN.texcoord1;
#if defined(DYNAMICLIGHTMAP_ON) || defined(UNITY_PASS_META)
    o.uv2 = IN.texcoord2;
#endif
#ifdef _TANGENT_TO_WORLD
    o.tangent = IN.tangent;
#endif
    return o;
}

DecalData PackVaryingsToDecalData(VaryingsDecal IN)
{
    DecalData o;
    o.normalOS = IN.normalOS;
    o.uv0 = IN.uv0;
    return o;
}

// -------------------------------------------------- //
//                      SAMPLING                      //
// -------------------------------------------------- //

float4 SampleDecal(DecalData IN, Texture2D decal, float4 defaultColor)
{
    float4 tex = decal.Sample(_Linear_Clamp_sampler, IN.uv0.xy / IN.uv0.w);
    float4 col;
    if(_Axis == 2 || _Axis == 3) // Y
        col = lerp(defaultColor, tex, (1 - saturate(IN.uv0.z)) * abs(IN.normalOS.y));
    else if(_Axis == 4 || _Axis == 5) // Z
        col = lerp(defaultColor, tex, (1 - saturate(IN.uv0.z)) * abs(IN.normalOS.z));
    else // X
        col = lerp(defaultColor, tex, (1 - saturate(IN.uv0.z)) * abs(IN.normalOS.x));
    return col;
}

// -------------------------------------------------- //
//                       SURFACE                      //
// -------------------------------------------------- //

struct DecalSurface
{
    float3 Albedo;
    float3 Normal;
    float3 Specular;
    float Smoothness;
    float3 Emission;
    float Alpha;
};

DecalSurface InitializeDecalSurface()
{
    DecalSurface surface;
    surface.Albedo = float3(1,1,1);
    surface.Normal = float3(0,0,1);
    surface.Specular = float3(0,0,0);
    surface.Smoothness = 0.5;
    surface.Emission = float3(0,0,0);
    surface.Alpha = 1;
    return surface;
}

// -------------------------------------------------- //
//                   SURFACE SAMPLE                   //
// -------------------------------------------------- //

float3 SampleNormal(DecalSurface surface, DecalData decalData, float4 tangentToWorld[3])
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
    return NormalizePerPixelNormal(tangent * surface.Normal.x + binormal * surface.Normal.y + normal * surface.Normal.z); // @TODO: see if we can squeeze this normalize on SM2.0 as well
#else
    return normalize(tangentToWorld[2].xyz);
#endif
}

half3 SampleEmission(DecalSurface surface)
{
#ifndef _EMISSION
    return 0;
#else
    return surface.Emission;
#endif
}

inline FragmentCommonData SurfaceToCommonData (DecalSurface surface, DecalData decalData,
     float4 tangentToWorld[3], float3 i_posWorld, float3 i_eyeVec)
{
    // AlphaTest
    #if defined(_ALPHATEST_ON)
        clip (surface.Alpha - _Cutoff);
    #endif

    // Calculate Diffuse color
    half oneMinusReflectivity;
    half3 diffColor = EnergyConservationBetweenDiffuseAndSpecular (surface.Albedo, surface.Specular, oneMinusReflectivity);

    // Convert to CommonData
    FragmentCommonData o = (FragmentCommonData)0;
    o.diffColor = PreMultiplyAlpha (diffColor, surface.Alpha, o.oneMinusReflectivity, o.alpha);
    o.normalWorld = SampleNormal(surface, decalData, tangentToWorld);
    o.specColor = surface.Specular;
    o.smoothness = surface.Smoothness;
    o.oneMinusReflectivity = oneMinusReflectivity;
    o.eyeVec = NormalizePerPixelNormal(i_eyeVec);
    o.posWorld = i_posWorld;

    return o;
}

// -------------------------------------------------- //
//                       VERTEX                       //
// -------------------------------------------------- //

VertexOutputForwardBase VertexDecal (AttributesDecal v, out DecalData decalData)
{
    // Define VaryingsDecal
    VaryingsDecal o;
    o.positionWS = mul(unity_ObjectToWorld, v.vertex);
    o.positionCS = UnityObjectToClipPos(v.vertex);
    o.normalOS = mul(v.normal, UNITY_MATRIX_M);
#ifdef _TANGENT_TO_WORLD
    o.tangentOS = v.tangent;
#endif
    o.uv0 = mul (unity_Projector, v.vertex);
    o.uv1 = v.texcoord0;
#ifdef _APPLYFOG
    UNITY_TRANSFER_FOG(o,o.positionCS);
#endif
    
    // Pack to VertexInput
    VertexInput vertexInput = PackAttributesToVertexInput(v);

    // Pack to VertexOutput
    VertexOutputForwardBase vertexOutput = PackVaryingsToLegacy(o);
    vertexOutput.ambientOrLightmapUV = VertexGIForward(vertexInput, o.positionWS, o.normalOS);

    // Pack to DecalData
    decalData = PackVaryingsToDecalData(o);

    // Finalize
    return vertexOutput;
}

#endif
