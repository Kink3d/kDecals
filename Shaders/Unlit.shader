Shader "kDecals/Unlit" 
{
	Properties 
	{
		// Surface Options
		[HideInInspector] _Blend("__blend", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
		[HideInInspector] _AlphaClip("__clip", Float) = 0.0
		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

		// Surface Inputs
		_Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _BaseTex("Albedo", 2D) = "white" {}
	}
	Subshader 
	{
		Tags {"Queue"="Transparent"}
		Pass 
		{
			// Render State
			Blend[_SrcBlend][_DstBlend]
			ZWrite Off
			Offset -1, -1

			CGPROGRAM
			#pragma vertex Vertex
			#pragma fragment Fragment

			// Variants
			#pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _ALPHAPREMULTIPLY_ON

			// Includes
			#include "Packages/com.kink3d.decals/ShaderLibrary/Unlit.hlsl"

			// Fragment
			half4 Fragment(Varyings input) : SV_Target
			{
				// ColorAlpha
				half4 colorAlpha = SampleDecalTexture(_BaseTex, input.positionPS);
				half3 color = colorAlpha.rgb * _Color.rgb;
				half alpha = colorAlpha.a * _Color.a;

				// AlphaClip
				half cutoff = _Cutoff;

				return FragmentUnlit(input, color, alpha, cutoff);
			}
			ENDCG
		}
	}
	CustomEditor "kTools.Decals.Editor.UnlitGUI"
}
