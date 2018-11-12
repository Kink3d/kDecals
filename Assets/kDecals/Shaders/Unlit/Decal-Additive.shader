Shader "Hidden/kDecals/Unlit/Additive" 
{
	Properties 
	{
		[HideInInspector] _Axis ("Axis", Int) = 0
		_DecalTex ("Decal", 2D) = "white" {}
	}
	Subshader 
	{
		Tags {"Queue"="Transparent"}
		Pass 
		{
			Blend One One
			Offset -1, -1

			CGPROGRAM
			#pragma vertex VertexDecal
			#pragma fragment FragmentDecalAdditive
			#include "../../ShaderLibrary/DecalInput.hlsl"

			float4 FragmentDecalAdditive (VaryingsDecal IN) : SV_Target
			{
				return SampleDecal(IN, float4(0,0,0,1));
			}
			
			ENDCG
		}
	}
}