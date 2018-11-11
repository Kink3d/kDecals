Shader "Hidden/kDecals/Unlit/AlphaClip" 
{
	Properties 
	{
		_DecalTex ("Decal", 2D) = "white" {}
		_Threshold ("Threshold", float) = 0.5
	}
	Subshader 
	{
		Tags {"Queue"="Transparent"}
		Pass 
		{
			Offset -1, -1

			CGPROGRAM
			#pragma vertex VertexDecal
			#pragma fragment FragmentDecalAlphaClip
			#include "../ShaderLibrary/DecalInput.hlsl"

			float _Threshold;

			float4 FragmentDecalAlphaClip (VaryingsDecal IN) : SV_Target
			{
				float4 col = SampleDecal(IN, float4(0,0,0,0));
				if(col.a < _Threshold)
					discard;
				return float4(col.rgb, 1);
			}
			
			ENDCG
		}
	}
}