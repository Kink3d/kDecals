using UnityEngine;
using kTools.ShaderUtil;

namespace kTools.Decals
{
    public class AdditiveDecalDefinition : kDecalDefinition
    {
        public override void DefineDecal(out DecalDefinitionContext context)
        {
            context = new DecalDefinitionContext()
            {
                name = "Additive",
                shader = "Hidden/SimpleDecals/Additive",
            };

            context.AddShaderProperty(new TextureProperty(
                "Decal",
                "_DecalTex",
                null
            ));
        }
    }
}
