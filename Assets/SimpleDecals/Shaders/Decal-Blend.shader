Shader "Hidden/SimpleDecals/Blend" 
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
			Blend SrcAlpha OneMinusSrcAlpha
			Offset -1, -1

			CGPROGRAM
			#pragma vertex VertexDecal
			#pragma fragment FragmentDecalBlend
			//#pragma multi_compile_fog
			#include "UnityCG.cginc"
			#include "../ShaderLibrary/DecalInput.hlsl"

			float4 FragmentDecalBlend (VaryingsDecal IN) : SV_Target
			{
				float4 col = SampleDecal(IN, float4(0,0,0,0));
				//UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(1,1,1,1));
				return col;
			}
			
			ENDCG
		}
	}
}