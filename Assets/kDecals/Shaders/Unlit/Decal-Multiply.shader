Shader "Hidden/kDecals/Unlit/Multiply" 
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
			Blend DstColor Zero
			Offset -1, -1

			CGPROGRAM
			#pragma vertex VertexDecal
			#pragma fragment FragmentDecalMultiply
			#include "../ShaderLibrary/DecalInput.hlsl"

			float4 FragmentDecalMultiply (VaryingsDecal IN) : SV_Target
			{
				return SampleDecal(IN, float4(1,1,1,1));
			}
			
			ENDCG
		}
	}
}