Shader "Hidden/kDecals/Unlit/Blend" 
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
			Blend SrcAlpha OneMinusSrcAlpha
			Offset -1, -1

			CGPROGRAM
			#pragma shader_feature _FOG
			#pragma multi_compile_fog
			
			#pragma vertex VertexDecal
			#pragma fragment FragmentDecal
			#include "../../ShaderLibrary/DecalInputUnlit.hlsl"

			void DefineDecalSurface(DecalData decalData, inout DecalSurfaceUnlit surface)
			{
				float4 color = SampleDecal(decalData, _DecalTex, float4(0,0,0,0));
				surface.Color = color.rgb;
				surface.Alpha = color.a;
			}

			#include "../../ShaderLibrary/DecalPassUnlit.hlsl"
			ENDCG
		}
	}
}