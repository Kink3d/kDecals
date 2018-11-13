#ifndef DECAL_PASS_UNLIT_INCLUDED
#define DECAL_PASS_UNLIT_INCLUDED

// -------------------------------------------------- //
//                       VERTEX                       //
// -------------------------------------------------- //

VaryingsDecal VertexDecal (AttributesDecal v)
{
    // Initiailize
    VaryingsDecal o;
    UNITY_INITIALIZE_OUTPUT(VaryingsDecal, o);

    // Common
    o.positionCS = UnityObjectToClipPos(v.vertex);
    o.normalOS = mul(v.normal, UNITY_MATRIX_M);
    o.uv0 = mul (unity_Projector, v.vertex);

    // Fog
#ifdef _FOG
    UNITY_TRANSFER_FOG(o, o.positionCS);
#endif

    // Finalize
    return o;
}

// -------------------------------------------------- //
//                      FRAGMENT                      //
// -------------------------------------------------- //

float4 FragmentDecal (VaryingsDecal IN) : SV_Target
{
    // Packing
    DecalData decalData = PackVaryingsToDecalData(IN);

    // Calculate Surface
    DecalSurfaceUnlit surface = InitializeDecalSurfaceUnlit();
    DefineDecalSurface(decalData, surface);				

    // Dither
    UNITY_APPLY_DITHER_CROSSFADE(IN.positionCS.xy);

    // Instancing
    UNITY_SETUP_INSTANCE_ID(IN);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

    // Fog
#ifdef _FOG
    UNITY_APPLY_FOG(IN.fogCoord, color.rgb);
#endif

    // Finalize
    return float4(surface.Color, surface.Alpha);
}

#endif
