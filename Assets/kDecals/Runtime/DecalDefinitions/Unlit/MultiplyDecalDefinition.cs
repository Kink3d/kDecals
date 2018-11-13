namespace kTools.Decals
{
    [DecalDefinition("Unlit/Multiply")]
    public class MultiplyDecalDefinition : DecalDefinition
    {
        public override void DefineDecal(out DecalDefinitionContext context)
        {
            context = new DecalDefinitionContext()
            {
                shader = "Hidden/kDecals/Unlit/Multiply",
            };

            context.AddDecalProperty(new KeywordDecalProperty(
                "Fog",
                "_FOG",
                false
            ));

            context.AddDecalProperty(new TextureDecalProperty(
                "Decal",
                "_DecalTex",
                null
            ));
        }
    }
}
