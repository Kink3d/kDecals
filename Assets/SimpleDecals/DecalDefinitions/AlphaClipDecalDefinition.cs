namespace kTools.Decals
{
    [DecalDefinition("Unlit/Alpha Clip")]
    public class AlphaClipDecalDefinition : DecalDefinition
    {
        public override void DefineDecal(out DecalDefinitionContext context)
        {
            context = new DecalDefinitionContext()
            {
                shader = "Hidden/kDecals/Unlit/AlphaClip",
            };

            context.AddShaderProperty(new TextureDecalProperty(
                "Decal",
                "_DecalTex",
                null
            ));

            context.AddShaderProperty(new FloatDecalProperty(
                "Threshold",
                "_Threshold",
                0.5f
            ));
        }
    }
}
