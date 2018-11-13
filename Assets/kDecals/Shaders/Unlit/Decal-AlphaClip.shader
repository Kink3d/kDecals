Shader "Hidden/kDecals/Unlit/AlphaClip" 
{
	Properties 
	{
		[HideInInspector] _Axis ("Axis", Int) = 0
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
			#pragma shader_feature _FOG
			#pragma multi_compile_fog
			
			#pragma vertex VertexDecal
			#pragma fragment FragmentDecal
			#include "../../ShaderLibrary/DecalInputUnlit.hlsl"

			float _Threshold;

			void DefineDecalSurface(DecalData decalData, inout DecalSurfaceUnlit surface)
			{
				float4 color = SampleDecal(decalData, _DecalTex, float4(0,0,0,0));
				surface.Color = color.rgb;
				surface.Alpha = color.a;

				if(color.a < _Threshold)
					discard;
			}

			#include "../../ShaderLibrary/DecalPassUnlit.hlsl"
			ENDCG
		}
	}
}