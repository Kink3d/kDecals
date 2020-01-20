Shader "kDecals/Lit" 
{
	Properties 
	{
		// Surface Options
        [HideInInspector] _WorkflowMode("WorkflowMode", Float) = 1.0
		[HideInInspector] _Blend("__blend", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
		[HideInInspector] _AlphaClip("__clip", Float) = 0.0
		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

		// Surface Inputs
		_Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _BaseTex("Albedo", 2D) = "white" {}
		_Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        _MetallicGlossTex("Metallic", 2D) = "white" {}
        _SpecColor("Specular", Color) = (0.2, 0.2, 0.2)
        _SpecGlossTex("Specular", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
        _GlossMapScale("Smoothness Scale", Range(0.0, 1.0)) = 1.0
        _BumpTex("Normal Map", 2D) = "bump" {}
		_BumpScale("Scale", Float) = 1.0
		_OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
        _OcclusionTex("Occlusion", 2D) = "white" {}
        _EmissionColor("Color", Color) = (0,0,0)
        _EmissionTex("Emission", 2D) = "white" {}
		
		// Advanced Options
		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
        [ToggleOff] _EnvironmentReflections("Environment Reflections", Float) = 1.0
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
			#pragma target 3.0
			#pragma vertex Vertex
			#pragma fragment Fragment

			// Variants
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _ALPHAPREMULTIPLY_ON
			#pragma shader_feature _METALLICSPECGLOSSMAP
            #pragma shader_feature _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature _ENVIRONMENTREFLECTIONS_OFF
            #pragma shader_feature _SPECULAR_SETUP
			#pragma shader_feature _NORMALMAP

			// Includes
			#include "Packages/com.kink3d.decals/ShaderLibrary/Lit.hlsl"

			// Fragment
			half4 Fragment(Varyings input) : SV_Target
			{
				// ColorAlpha
				half4 colorAlpha = SampleDecalTexture(_BaseTex, input.positionPS);
				half3 color = colorAlpha.rgb * _Color.rgb;
				half alpha = colorAlpha.a * _Color.a;
				
				// MetallicSpecular
				half4 specGloss = SampleMetallicSpecGloss(input.positionPS);
				half3 specularColor = specGloss.rgb;
				half smoothness = specGloss.a;

				half3 normal = UnpackScaleNormal(SampleDecalTexture(_BumpTex, input.positionPS), _BumpScale);
				half occlusion = SampleDecalTexture(_OcclusionTex, input.positionPS) * _OcclusionStrength;
				half3 emission = SampleDecalTexture(_EmissionTex, input.positionPS) * _EmissionColor;				

				// AlphaClip
				half cutoff = _Cutoff;

				return FragmentLit (input, color, specularColor, smoothness, normal, 
    				emission, occlusion, alpha, cutoff);
			}
			ENDCG
		}
	}
	CustomEditor "kTools.Decals.Editor.LitGUI"
}
