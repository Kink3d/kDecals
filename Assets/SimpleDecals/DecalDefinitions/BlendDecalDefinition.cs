namespace kTools.Decals
{
    [DecalDefinition("Unlit/Blend")]
    public class BlendDecalDefinition : DecalDefinition
    {
        public override void DefineDecal(out DecalDefinitionContext context)
        {
            context = new DecalDefinitionContext()
            {
                shader = "Hidden/kDecals/Unlit/Blend",
            };

            context.AddShaderProperty(new TextureDecalProperty(
                "Decal",
                "_DecalTex",
                null
            ));
        }
    }
}
