Shader "Hidden/kDecals/Unlit/Blend" 
{
	Properties 
	{
		_DecalTex ("Decal", 2D) = "white" {}
	}
	Subshader 
	{
		Tags {"Queue"="Transparent"}
		Pass 
		{
			Blend SrcAlpha OneMinusSrcAlpha
			Offset -1, -1

			CGPROGRAM
			#pragma vertex VertexDecal
			#pragma fragment FragmentDecalBlend
			#include "../ShaderLibrary/DecalInput.hlsl"

			float4 FragmentDecalBlend (VaryingsDecal IN) : SV_Target
			{
				return SampleDecal(IN, float4(0,0,0,0));
			}
			
			ENDCG
		}
	}
}