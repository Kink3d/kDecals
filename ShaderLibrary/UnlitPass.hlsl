#ifndef DECAL_UNLIT_PASS_INCLUDED
#define DECAL_UNLIT_PASS_INCLUDED

// -------------------------------------
// Includes
#include "Packages/com.kink3d.decals/ShaderLibrary/Core.hlsl"

// -------------------------------------
// Structs
struct Attributes
{
    float4 positionOS       : POSITION;
    float3 normalOS         : NORMAL;
    float4 texcoord         : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
#ifdef _DECALTYPE_PROJECTION
    float4 positionPS               : TEXCOORD0;
#else
    float4 uv                       : TEXCOORD0;
#endif

    float fogCoord  		: TEXCOORD1;
    float3 normalWS         : TEXCOORD2;
    float4 vertex 			: SV_POSITION;

    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

// -------------------------------------
// Vertex
Varyings vert(Attributes input)
{
    Varyings output = (Varyings)0;

    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
    output.vertex = vertexInput.positionCS;

#ifdef _DECALTYPE_PROJECTION
    output.positionPS = TransformObjectToProjection(input.positionOS);
#else
    output.uv = input.texcoord;
#endif

    output.fogCoord = ComputeFogFactor(vertexInput.positionCS.z);
    output.normalWS = TransformObjectToWorldNormal(input.normalOS);

    return output;
}

// -------------------------------------
// Fragment
half4 frag(Varyings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

#ifdef _DECALTYPE_PROJECTION
    if(ClampProjection(input.positionPS, input.normalWS))
        discard;
#endif
    
#ifdef _DECALTYPE_PROJECTION
    float4 uv = input.positionPS;
#else
    float4 uv = input.uv;
#endif

    // Apply Scale & Offset
    uv.xy = TRANSFORM_TEX(uv, _BaseMap);

    half4 texColor = SAMPLE_DECAL2D(_BaseMap, sampler_BaseMap, uv);
    half3 color = texColor.rgb * _BaseColor.rgb;
    half alpha = texColor.a * _BaseColor.a;
    AlphaDiscard(alpha, _Cutoff);

    #ifdef _ALPHAPREMULTIPLY_ON
        color *= alpha;
    #endif

    half4 finalColor = half4(color, alpha);
    finalColor.rgb = MixFog(finalColor.rgb, input.fogCoord);    
    return finalColor;
}

#endif
