Shader "Hidden/kDecals/PBR/Blend" 
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
		_EmissionColor ("Emission", Color) = (0,0,0,0)
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

			#pragma target 3.0
			#pragma vertex VertexDecal
			#pragma fragment FragmentDecal

			#define _ALPHABLEND_ON 1
			#define _GLOSSYREFLECTIONS_OFF 1

			#include "../../ShaderLibrary/DecalInputLit.hlsl"

			void DefineDecalSurface(DecalData decalData, inout DecalSurface surface)
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

			// TODO
			// - Move to DecalLit when inheritance works
			float4 FragmentDecal (VertexOutputForwardBase i, DecalData decalData) : SV_Target
			{
				// Calculate Surface
				DecalSurface surface = InitializeDecalSurface();
				DefineDecalSurface(decalData, surface);				

				// Dither
				UNITY_APPLY_DITHER_CROSSFADE(i.pos.xy);

				// Calculate CommonData
				FragmentCommonData commonData = SurfaceToCommonData(surface, decalData, i.tangentToWorldAndPackedData, IN_WORLDPOS(i), i.eyeVec);
				
				// Instancing
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

				// Lighting
				UnityLight mainLight = MainLight ();
				UNITY_LIGHT_ATTENUATION(atten, IN, commonData.posWorld);
				float occlusion = 1;
				UnityGI gi = FragmentGI (commonData, occlusion, i.ambientOrLightmapUV, atten, mainLight);
				half4 color = UNITY_BRDF_PBS (commonData.diffColor, commonData.specColor, commonData.oneMinusReflectivity, 
					commonData.smoothness, commonData.normalWorld, -commonData.eyeVec, gi.light, gi.indirect);
				color.rgb += SampleEmission(surface);

				// Finalize
				UNITY_APPLY_FOG(i.fogCoord, color.rgb);
				return OutputForward (color, commonData.alpha);
			}
			
			ENDCG
		}
	}
}