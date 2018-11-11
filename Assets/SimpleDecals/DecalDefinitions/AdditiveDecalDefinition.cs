namespace kTools.Decals
{
    [DecalDefinition("Unlit/Additive")]
    public class AdditiveDecalDefinition : DecalDefinition
    {
        public override void DefineDecal(out DecalDefinitionContext context)
        {
            context = new DecalDefinitionContext()
            {
                shader = "Hidden/kDecals/Unlit/Additive",
            };

            context.AddDecalProperty(new TextureDecalProperty(
                "Decal",
                "_DecalTex",
                null
            ));
        }
    }
}
