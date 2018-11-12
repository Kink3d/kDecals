namespace kTools.Decals
{
    [DecalDefinition("PBR/Blend")]
    public class PBRBlendDecalDefinition : DecalDefinition
    {
        public override void DefineDecal(out DecalDefinitionContext context)
        {
            context = new DecalDefinitionContext()
            {
                shader = "Hidden/kDecals/PBR/Blend",
            };

            context.AddDecalProperty(new TextureDecalProperty(
                "Albedo",
                "_MainTex",
                null
            ));
        }
    }
}
