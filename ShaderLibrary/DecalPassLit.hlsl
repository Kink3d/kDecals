#ifndef DECAL_PASS_LIT_INCLUDED
#define DECAL_PASS_LIT_INCLUDED

// -------------------------------------------------- //
//                   SURFACE UTILS                    //
// -------------------------------------------------- //

float3 SampleNormal(DecalSurfaceLit surface, DecalData decalData, float4 tangentToWorld[3])
{
#ifdef _NORMALMAP
    // Split TangentToWorld
    half3 tangent = tangentToWorld[0].xyz;
    half3 binormal = tangentToWorld[1].xyz;
    half3 normal = tangentToWorld[2].xyz;

    // Orthonormalize
    #if UNITY_TANGENT_ORTHONORMALIZE
        normal = NormalizePerPixelNormal(normal);
        tangent = normalize (tangent - normal * dot(tangent, normal));
        half3 newB = cross(normal, tangent);
        binormal = newB * sign (dot (newB, binormal));
    #endif
    
    // Finalize
    return NormalizePerPixelNormal(tangent * surface.Normal.x + binormal * surface.Normal.y + normal * surface.Normal.z); // @TODO: see if we can squeeze this normalize on SM2.0 as well
#else
    return normalize(tangentToWorld[2].xyz);
#endif
}

half3 SampleEmission(DecalSurfaceLit surface)
{
#ifndef _EMISSION
    return 0;
#else
    return surface.Emission;
#endif
}

// -------------------------------------------------- //
//                       VERTEX                       //
// -------------------------------------------------- //

VertexInput PackAttributesToVertexInput(AttributesDecal IN)
{
    // Initialize
    VertexInput o;
    UNITY_INITIALIZE_OUTPUT(VertexInput, o);

    // Copy data
    o.vertex = IN.vertex;
    o.normal = IN.normal;
    o.uv0 = IN.texcoord0;
    o.uv1 = IN.texcoord1;
#if defined(DYNAMICLIGHTMAP_ON) || defined(UNITY_PASS_META)
    o.uv2 = IN.texcoord2;
#endif
#ifdef _TANGENT_TO_WORLD
    o.tangent = IN.tangent;
#endif

    // Finalize
    return o;
}

VaryingsDecal VertexDecal (AttributesDecal v)
{
    // Initiailize
    VaryingsDecal o;
    UNITY_INITIALIZE_OUTPUT(VaryingsDecal, o);

    // Common
    o.positionCS = UnityObjectToClipPos(v.vertex);
    o.normalOS = mul(v.normal, UNITY_MATRIX_M);
    o.uv0 = mul (unity_Projector, v.vertex);

    // Extra lighting variables
#ifdef _TANGENT_TO_WORLD
    o.tangentOS = v.tangent;
#endif
    o.uv1 = v.texcoord0;
    o.positionWS = mul(unity_ObjectToWorld, v.vertex);
    o.viewDir = float4(NormalizePerVertexNormal(o.positionWS.xyz - _WorldSpaceCameraPos), 1);

    // TangentToWorld
#ifdef _TANGENT_TO_WORLD
    float4 tangentWorld = float4(UnityObjectToWorldDir(o.tangentOS.xyz), o.tangentOS.w);
    float3x3 tangentToWorld = CreateTangentToWorldPerVertex(o.normalOS, tangentWorld.xyz, tangentWorld.w);
    o.tangentToWorld[0].xyz = tangentToWorld[0];
    o.tangentToWorld[1].xyz = tangentToWorld[1];
    o.tangentToWorld[2].xyz = tangentToWorld[2];
#else
    o.tangentToWorld[0].xyz = 0;
    o.tangentToWorld[1].xyz = 0;
    o.tangentToWorld[2].xyz = o.normalOS;
#endif

    // Shadow Receiving
    UNITY_TRANSFER_LIGHTING(o, IN.uv1);

    // Fog
#ifdef _FOG
    UNITY_TRANSFER_FOG(o, o.positionCS);
#endif
    
    // Pack to VertexInput to Lightmap/SH coords
    VertexInput vertexInput = PackAttributesToVertexInput(v);
    o.ambientOrLightmapUV = VertexGIForward(vertexInput, o.positionWS, o.normalOS);

    // Finalize
    return o;
}

// -------------------------------------------------- //
//                      FRAGMENT                      //
// -------------------------------------------------- //

inline FragmentCommonData PackSurfaceToCommonData (DecalSurfaceLit surface, DecalData decalData,
     float4 tangentToWorld[3], float3 positionWS, float3 viewDir)
{
    // AlphaTest
    #ifdef _ALPHATEST
        if(surface.Alpha < _Threshold)
			discard;
    #endif

    // Convert to CommonData
    FragmentCommonData o = (FragmentCommonData)0;
    half oneMinusReflectivity;
    o.diffColor = EnergyConservationBetweenDiffuseAndSpecular (surface.Albedo, surface.Specular, oneMinusReflectivity);
    o.normalWorld = SampleNormal(surface, decalData, tangentToWorld);
    o.specColor = surface.Specular;
    o.smoothness = surface.Smoothness;
    o.alpha = surface.Alpha;
    o.oneMinusReflectivity = oneMinusReflectivity;
    o.eyeVec = NormalizePerPixelNormal(-viewDir);
    o.posWorld = positionWS;

    return o;
}

float4 FragmentDecal (VaryingsDecal IN) : SV_Target
{
    // Packing
    DecalData decalData = PackVaryingsToDecalData(IN);

    // Calculate Surface
    DecalSurfaceLit surface = InitializeDecalSurfaceLit();
    DefineDecalSurface(decalData, surface);				

    // Dither
    UNITY_APPLY_DITHER_CROSSFADE(IN.positionCS.xy);
    
    // Instancing
    UNITY_SETUP_INSTANCE_ID(IN);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

    // Calculate CommonData
    FragmentCommonData commonData = PackSurfaceToCommonData(surface, decalData, IN.tangentToWorld, IN.positionWS, IN.viewDir);

    // Lighting
    UnityLight mainLight = MainLight ();
    UNITY_LIGHT_ATTENUATION(atten, IN, commonData.posWorld);
    half occlusion = 1;
    UnityGI gi = FragmentGI (commonData, occlusion, IN.ambientOrLightmapUV, atten, mainLight);
    half4 color = UNITY_BRDF_PBS (commonData.diffColor, commonData.specColor, commonData.oneMinusReflectivity, 
        commonData.smoothness, commonData.normalWorld, commonData.eyeVec, gi.light, gi.indirect);
    color.rgb += SampleEmission(surface);

    // Fog
#ifdef _FOG
    UNITY_APPLY_FOG(IN.fogCoord, color.rgb);
#endif

    // Alpha
    half alpha = commonData.alpha;
#ifdef _ALPHATEST
    alpha = 1;
#endif

    // Finalize
    return half4(color.rgb, alpha);
}

#endif
