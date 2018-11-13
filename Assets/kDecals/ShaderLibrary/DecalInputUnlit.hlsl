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
    float3 Color;
    float Alpha;
};

DecalSurfaceUnlit InitializeDecalSurfaceUnlit()
{
    DecalSurfaceUnlit surface;
    surface.Color = float3(1,1,1);
    surface.Alpha = 1;
    return surface;
}

#endif
