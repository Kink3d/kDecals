Shader "Hidden/SimpleDecals/Additive" 
{
	Properties 
	{
		_DecalTex ("Decal", 2D) = "white" {}
		[HideInInspector] _Axis ("Axis", Int) = 0
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
			//#pragma multi_compile_fog
			#include "UnityCG.cginc"
			#include "../ShaderLibrary/DecalInput.hlsl"

			float4 FragmentDecalAdditive (VaryingsDecal IN) : SV_Target
			{
				float4 col = SampleDecal(IN, float4(0,0,0,1));
				//UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(1,1,1,1));
				return col;
			}
			
			ENDCG
		}
	}
}