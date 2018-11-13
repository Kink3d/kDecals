#ifndef DECAL_INPUT_UNLIT_INCLUDED
#define DECAL_INPUT_UNLIT_INCLUDED

// -------------------------------------------------- //
//                      INCLUDES                      //
// -------------------------------------------------- //

#include "DecalInput.hlsl"

// -------------------------------------------------- //
//                      UNIFORMS                      //
// -------------------------------------------------- //

Texture2D _DecalTex;

// -------------------------------------------------- //
//                       SURFACE                      //
// -------------------------------------------------- //

struct DecalSurfaceUnlit
{
    half3 Color;
    half Alpha;
};

DecalSurfaceUnlit InitializeDecalSurfaceUnlit()
{
    DecalSurfaceUnlit surface;
    surface.Color = half3(1,1,1);
    surface.Alpha = 1;
    return surface;
}

#endif
