Shader "Hidden/kDecals/PBR/Blend" 
{
	Properties 
	{
		[HideInInspector] _Axis ("Axis", Int) = 0
		_MainTex ("Albedo", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		_BumpMap ("Normal", 2D) = "bump" {}
		_SpecGlossMap ("Specular Map", 2D) = "white" {}
		_SpecColor ("Specular Color", Color) = (0,0,0,0)
		_Glossiness ("Smoothness", Range(0, 1)) = 0.5
		_OcclusionMap ("Occlusion Map", 2D) = "white" {}
		_OcclusionStrength ("Occlusion", Range(0, 1)) = 0
		_EmissionMap ("Emission Map", 2D) = "white" {}
		_OcclusionColor ("Emission", Color) = (0,0,0,0)

		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
        [ToggleOff] _GlossyReflections("Glossy Reflections", Float) = 0.0
	}
	Subshader 
	{
		Tags {"Queue"="Transparent"}
		Pass 
		{
			Blend SrcAlpha OneMinusSrcAlpha
			Offset -1, -1

			CGPROGRAM
			#pragma target 3.0

			#pragma shader_feature _NORMALMAP
            #pragma shader_feature _EMISSION
            #pragma shader_feature _SPECGLOSSMAP
            #pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature _ _GLOSSYREFLECTIONS_OFF

			#pragma vertex VertexDecalLit
			#pragma fragment FragmentDecalBlend
			#include "UnityStandardCore.cginc"
			#include "../../ShaderLibrary/DecalInput.hlsl"

			Texture2D _AlbedoTex;

			// -------------------------------------------------- //
			//                     ATTRIBUTES                     //
			// -------------------------------------------------- //

			struct AttributesDecalLit
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 texcoord0 : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
			};

			// -------------------------------------------------- //
			//                      VARYINGS                      //
			// -------------------------------------------------- //

			struct VaryingsDecalLit 
			{
				float4 positionCS : SV_POSITION;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 uv0 : TEXCOORD0;
				float4 uv1 : TEXCOORD1;
				float4 positionWS : TEXCOORD3;
			#ifdef _APPLYFOG
				UNITY_FOG_COORDS(2)
			#endif
			};

			// -------------------------------------------------- //
			//                       VERTEX                       //
			// -------------------------------------------------- //

			VaryingsDecalLit VertexDecalLit (AttributesDecalLit v)
			{
				VaryingsDecalLit o;
				o.positionWS = mul(unity_ObjectToWorld, v.vertex);
				o.positionCS = UnityObjectToClipPos(v.vertex);
				o.normalOS = mul(v.normal, UNITY_MATRIX_M);
				o.tangentOS = v.tangent;
				o.uv0 = mul (unity_Projector, v.vertex);
				o.uv1 = v.texcoord1;
			#ifdef _APPLYFOG
				UNITY_TRANSFER_FOG(o,o.positionCS);
			#endif
				return o;
			}

			VertexOutputForwardBase PackVaryingsToLegacy(VaryingsDecalLit IN)
			{
				VertexOutputForwardBase o;
				UNITY_INITIALIZE_OUTPUT(VertexOutputForwardBase, o);

				#if UNITY_REQUIRE_FRAG_WORLDPOS
					#if UNITY_PACK_WORLDPOS_WITH_TANGENT
						o.tangentToWorldAndPackedData[0].w = IN.positionWS.x;
						o.tangentToWorldAndPackedData[1].w = IN.positionWS.y;
						o.tangentToWorldAndPackedData[2].w = IN.positionWS.z;
					#else
						o.posWorld = IN.positionWS.xyz;
					#endif
				#endif
				o.pos = IN.positionCS;

				o.tex = IN.uv0;
				o.eyeVec = float4(NormalizePerVertexNormal(IN.positionWS.xyz - _WorldSpaceCameraPos), 1);
				#ifdef _TANGENT_TO_WORLD
					float4 tangentWorld = float4(UnityObjectToWorldDir(IN.tangentOS.xyz), IN.tangentOS.w);

					float3x3 tangentToWorld = CreateTangentToWorldPerVertex(IN.normalOS, tangentWorld.xyz, tangentWorld.w);
					o.tangentToWorldAndPackedData[0].xyz = tangentToWorld[0];
					o.tangentToWorldAndPackedData[1].xyz = tangentToWorld[1];
					o.tangentToWorldAndPackedData[2].xyz = tangentToWorld[2];
				#else
					o.tangentToWorldAndPackedData[0].xyz = 0;
					o.tangentToWorldAndPackedData[1].xyz = 0;
					o.tangentToWorldAndPackedData[2].xyz = IN.normalOS;
				#endif

				//We need this for shadow receving
				UNITY_TRANSFER_LIGHTING(o, IN.uv1);

				o.ambientOrLightmapUV = float4(1,1,1,1);//VertexGIForward(v, posWorld, IN.normalOS);

				#ifdef _PARALLAXMAP
					TANGENT_SPACE_ROTATION;
					half3 viewDirForParallax = mul (rotation, ObjSpaceViewDir(v.vertex));
					o.tangentToWorldAndPackedData[0].w = viewDirForParallax.x;
					o.tangentToWorldAndPackedData[1].w = viewDirForParallax.y;
					o.tangentToWorldAndPackedData[2].w = viewDirForParallax.z;
				#endif

				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}

			float4 FragmentDecalBlend (VaryingsDecalLit IN) : SV_Target
			{
				//SurfaceOutputStandard o;
				//float4 col = float4(1,1,1,1);//SampleDecal(IN, _AlbedoTex, float4(0,0,0,0));
				//o.Albedo = col.rgb;
				//o.Alpha = col.a;

				VertexOutputForwardBase i = PackVaryingsToLegacy(IN);

				UNITY_APPLY_DITHER_CROSSFADE(i.pos.xy);
				FRAGMENT_SETUP(s)
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

				UnityLight mainLight = MainLight ();
				UNITY_LIGHT_ATTENUATION(atten, i, s.posWorld);

				half occlusion = Occlusion(i.tex.xy);
				UnityGI gi = FragmentGI (s, occlusion, i.ambientOrLightmapUV, atten, mainLight);

				half4 c = UNITY_BRDF_PBS (s.diffColor, s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, -s.eyeVec, gi.light, gi.indirect);
				c.rgb += Emission(i.tex.xy);

				UNITY_APPLY_FOG(i.fogCoord, c.rgb);
				return OutputForward (c, s.alpha);
			}
			
			ENDCG
		}
	}
}