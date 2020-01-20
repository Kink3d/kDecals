#ifndef DECAL_UNLIT_INCLUDED
#define DECAL_UNLIT_INCLUDED

// Includes
#include "Packages/com.kink3d.decals/ShaderLibrary/Core.hlsl"

// Attributes
struct Attributes
{
    float4 positionOS : POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

// Varyings
struct Varyings
{
    float4 positionCS : SV_POSITION;
    float4 positionPS : TEXCOORD0;
};

// Uniforms
Texture2D _BaseTex;
half4 _Color;
half _Cutoff;

// Vertex
Varyings Vertex (Attributes input)
{
    Varyings output;
    UNITY_INITIALIZE_OUTPUT(Varyings, output);
    
    output.positionCS = UnityObjectToClipPos(input.positionOS);
    output.positionPS = UnityObjectToProjectionPos(input.positionOS);
    return output;
}

// Fragment
half4 FragmentUnlit (Varyings input, half3 color, half alpha, half cutoff)
{
    color = AlphaPremultiply(color, alpha);
    AlphaClip(alpha, cutoff);
    UNITY_APPLY_DITHER_CROSSFADE(input.positionCS.xy);
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    half4 finalColor = half4(color, alpha);
    finalColor = ClampProjection(finalColor, input.positionPS);
    
    return finalColor;
}

#endif
