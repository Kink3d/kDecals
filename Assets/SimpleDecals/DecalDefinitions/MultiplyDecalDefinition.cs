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

            context.AddShaderProperty(new TextureDecalProperty(
                "Decal",
                "_DecalTex",
                null
            ));
        }
    }
}
