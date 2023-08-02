Shader "kDecals/Unlit" 
{
	Properties 
	{
		// Surface Options
		[HideInInspector] _Surface("__surface", Float) = 1.0
		[HideInInspector] _Blend("__blend", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
		[HideInInspector] _AlphaClip("__clip", Float) = 0.0
		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

		// Surface Inputs
		_BaseMap("Albedo", 2D) = "white" {}
		_BaseColor("Color", Color) = (1.0, 1.0, 1.0, 1.0)
	}
	Subshader 
	{
		Tags { "RenderPipeline" = "UniversalPipeline" }
		Blend[_SrcBlend][_DstBlend]
		ZWrite Off
		Offset -1, -1

		Pass 
		{
			Tags { "LightMode" = "DecalForward" }

			HLSLPROGRAM
			// Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
			#pragma target 2.0

			#pragma vertex vert
            #pragma fragment frag

			// -------------------------------------
            // Material Keywords
			#pragma shader_feature _BLEND_ALPHA
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _ALPHAPREMULTIPLY_ON

			// -------------------------------------
            // Unity defined keywords
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

			// -------------------------------------
            // Includes
			#include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"
			#include "Packages/com.kink3d.decals/ShaderLibrary/UnlitPass.hlsl"
			
			ENDHLSL
		}
	}
	CustomEditor "kTools.Decals.Editor.UnlitGUI"
	FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
