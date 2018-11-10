using UnityEngine;
using kTools.ShaderUtil;

namespace kTools.Decals
{
    public class AdditiveDecalDefinition : DecalDefinition
    {
        public override void DefineDecal(out DecalDefinitionContext context)
        {
            context = new DecalDefinitionContext()
            {
                name = "Additive",
                shader = "Hidden/SimpleDecals/Additive",
            };

            context.AddShaderProperty(new TextureDecalProperty(
                "Decal",
                "_DecalTex",
                null
            ));
        }
    }
}
