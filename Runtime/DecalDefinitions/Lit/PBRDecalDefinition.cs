
using UnityEngine;

namespace kTools.Decals
{
    [DecalDefinition("Lit/PBR")]
    public class PBRDecalDefinition : DecalDefinition
    {
        public override void DefineDecal(out DecalDefinitionContext context)
        {
            context = new DecalDefinitionContext()
            {
                shader = "Hidden/kDecals/Lit/PBR",
            };

            context.AddDecalProperty(new KeywordDecalProperty(
                "Specular Map",
                "_SPECGLOSSMAP",
                false
            ));

            context.AddDecalProperty(new KeywordDecalProperty(
                "Normal Map",
                "_NORMALMAP",
                false
            ));

            context.AddDecalProperty(new KeywordDecalProperty(
                "Emission",
                "_EMISSION",
                false
            ));

            context.AddDecalProperty(new KeywordDecalProperty(
                "Alpha Clip",
                "_ALPHATEST",
                false
            ));

            context.AddDecalProperty(new KeywordDecalProperty(
                "Fog",
                "_FOG",
                true
            ));

            context.AddDecalProperty(new TextureDecalProperty(
                "Albedo",
                "_AlbedoTex",
                null
            ));

            context.AddDecalProperty(new ColorDecalProperty(
                "Color",
                "_Color",
                Color.white
            ));

            context.AddDecalProperty(new TextureDecalProperty(
                "Normal",
                "_NormalTex",
                null
            ));

            context.AddDecalProperty(new FloatDecalProperty(
                "Normal Scale",
                "_NormalScale",
                1.0f
            ));

            context.AddDecalProperty(new TextureDecalProperty(
                "Specular Map",
                "_SpecularTex",
                null
            ));

            context.AddDecalProperty(new ColorDecalProperty(
                "Specular Color",
                "_Specular",
                Color.black
            ));

            context.AddDecalProperty(new FloatDecalProperty(
                "Smoothness",
                "_Smoothness",
                0.5f
            ));

            context.AddDecalProperty(new TextureDecalProperty(
                "Emission Map",
                "_EmissionTex",
                null
            ));

            context.AddDecalProperty(new ColorDecalProperty(
                "Emission Color",
                "_EmissionColor",
                Color.black
            ));

            context.AddDecalProperty(new FloatDecalProperty(
                "Clip Threshold",
                "_Threshold",
                0.5f
            ));
        }
    }
}
