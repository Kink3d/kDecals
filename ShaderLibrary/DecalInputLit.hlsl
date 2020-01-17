#ifndef DECAL_INPUT_LIT_INCLUDED
#define DECAL_INPUT_LIT_INCLUDED

// -------------------------------------------------- //
//                       DEFINES                      //
// -------------------------------------------------- //

#define _GLOSSYREFLECTIONS_OFF 1
#define _LIGHTING 1

// -------------------------------------------------- //
//                      INCLUDES                      //
// -------------------------------------------------- //

#include "UnityStandardCore.cginc"
#include "DecalInput.hlsl"

// -------------------------------------------------- //
//                      UNIFORMS                      //
// -------------------------------------------------- //

Texture2D _AlbedoTex;
Texture2D _NormalTex;
Texture2D _SpecularTex;
Texture2D _EmissionTex;
half _NormalScale;
half4 _Specular;
half _Smoothness;
half _Threshold;

// -------------------------------------------------- //
//                       SURFACE                      //
// -------------------------------------------------- //

struct DecalSurfaceLit
{
    float3 Albedo;
    float3 Normal;
    float3 Specular;
    float Smoothness;
    float3 Emission;
    float Alpha;
};

DecalSurfaceLit InitializeDecalSurfaceLit()
{
    DecalSurfaceLit surface;
    surface.Albedo = float3(1,1,1);
    surface.Normal = float3(0,0,1);
    surface.Specular = float3(0,0,0);
    surface.Smoothness = 0.5;
    surface.Emission = float3(0,0,0);
    surface.Alpha = 1;
    return surface;
}

#endif
