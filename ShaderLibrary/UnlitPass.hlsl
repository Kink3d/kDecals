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
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float4 positionPS       : TEXCOORD0;
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
    output.positionPS = TransformObjectToProjection(input.positionOS);
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

    half4 texColor = SAMPLE_DECAL2D(_BaseMap, input.positionPS);
    half3 color = texColor.rgb * _BaseColor.rgb;
    half alpha = texColor.a * _BaseColor.a;
    AlphaDiscard(alpha, _Cutoff);

    #ifdef _ALPHAPREMULTIPLY_ON
        color *= alpha;
    #endif

    half4 finalColor = half4(color, alpha);
    finalColor.rgb = MixFog(finalColor.rgb, input.fogCoord);
    CLAMP_PROJECTION(finalColor, input.positionPS, input.normalWS);
    return finalColor;
}

#endif
