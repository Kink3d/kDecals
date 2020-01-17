Shader "Hidden/kDecals/Lit/Blinn" 
{
	Properties 
	{
		// Decal
		[HideInInspector] _Axis ("Axis", Int) = 0

		// Surface
		_AlbedoTex ("Albedo", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		_NormalTex ("Normal", 2D) = "bump" {}
		_NormalScale ("Normal Scale", Range(0, 1)) = 1
		_SpecularTex ("Specular Map", 2D) = "white" {}
		_Specular ("Specular Color", Color) = (0,0,0,0)
		_Smoothness ("Smoothness", Range(0, 1)) = 0.5
		_EmissionTex ("Emission Map", 2D) = "white" {}
		_EmissionColor ("Emission Color", Color) = (0,0,0,0)
		_Threshold ("Clip Threshold", Range(0, 1)) = 0.5
	}
	Subshader 
	{
		Tags {"Queue"="Transparent"}
		Pass 
		{
			Blend SrcAlpha OneMinusSrcAlpha
			Offset -1, -1

			CGPROGRAM
			#pragma shader_feature _NORMALMAP
			#pragma shader_feature _EMISSION
			#pragma shader_feature _SPECGLOSSMAP
			#pragma shader_feature _FOG
			#pragma shader_feature _ALPHATEST
			#pragma multi_compile_fog

			#pragma target 3.0
			#pragma vertex VertexDecal
			#pragma fragment FragmentDecal
			#define UNITY_BRDF_PBS BRDF3_Unity_PBS
			#include "../../ShaderLibrary/DecalInputLit.hlsl"

			void DefineDecalSurface(DecalData decalData, inout DecalSurfaceLit surface)
			{
				float4 c = SampleDecal(decalData, _AlbedoTex, float4(1,1,1,0)) * _Color;
				surface.Albedo = c.rgb;
				surface.Alpha = c.a;
				surface.Normal = UnpackScaleNormal(SampleDecal(decalData, _NormalTex, float4(0,0,0,0)), _NormalScale);

			#ifdef _SPECGLOSSMAP
				float4 specular = SampleDecal(decalData, _SpecularTex, float4(0,0,0,0));
				surface.Specular = specular.rgb;
				surface.Smoothness = specular.a;
			#else
				surface.Specular = _Specular.rgb;
				surface.Smoothness = _Smoothness;
			#endif
				
				surface.Emission = SampleDecal(decalData, _EmissionTex, float4(0,0,0,0)) * _EmissionColor;
			}

			#include "../../ShaderLibrary/DecalPassLit.hlsl"
			ENDCG
		}
	}
}