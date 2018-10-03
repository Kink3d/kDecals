Shader "Hidden/SimpleDecals/Multiply" 
{
	Properties 
	{
		[HideInInspector] _DecalTex ("Decal", 2D) = "white" {}
		[HideInInspector] _Axis ("Axis", Int) = 0
	}
	Subshader 
	{
		Tags {"Queue"="Transparent"}
		Pass 
		{
			ZWrite Off
			ColorMask RGB
			Blend DstColor Zero // Multiplicative
			Offset -1, -1

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fog
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};
			
			struct v2f 
			{
				float4 positionCS : SV_POSITION;
				float3 normalOS : NORMAL;
				float4 uv0 : TEXCOORD0;
				UNITY_FOG_COORDS(2)
			};
			
			float4x4 unity_Projector;
			Texture2D _DecalTex;
			SamplerState _Linear_Clamp_sampler;
			int _Axis;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.positionCS = UnityObjectToClipPos(v.vertex);
				o.normalOS = v.normal;
				o.uv0 = mul (unity_Projector, v.vertex);
				//UNITY_TRANSFER_FOG(o,o.positionCS);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 tex = _DecalTex.Sample(_Linear_Clamp_sampler, i.uv0.xy / i.uv0.w);
				fixed4 col;
				if(_Axis == 2 || _Axis == 3)
					col = lerp(fixed4(1,1,1,1), tex, saturate(sign(i.uv0.y) * abs(i.normalOS.y)));
				else if(_Axis == 4 || _Axis == 5)
					col = lerp(fixed4(1,1,1,1), tex, saturate(sign(i.uv0.z) * abs(i.normalOS.z)));
				else
					col = lerp(fixed4(1,1,1,1), tex, saturate(sign(i.uv0.x) * abs(i.normalOS.x)));
				//UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(1,1,1,1));
				return col;
			}
			ENDCG
		}
	}
}