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

#ifndef FOG_COLOR
    #define FOG_COLOR unity_FogColor
#endif

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
    float3 color = surface.Color;

    // Dither
    UNITY_APPLY_DITHER_CROSSFADE(IN.positionCS.xy);

    // Instancing
    UNITY_SETUP_INSTANCE_ID(IN);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

    // Fog
#ifdef _FOG
    UNITY_APPLY_FOG_COLOR(IN.fogCoord, color.rgb, FOG_COLOR);
#endif

    // Finalize
    return float4(color, surface.Alpha);
}

#endif
