using UnityEngine.Rendering.Universal;

namespace kTools.Decals
{
    [DisallowMultipleRendererFeature("kDecals")]
    sealed class KDecalRendererFeature : ScriptableRendererFeature
    {
#region Fields
        static KDecalRendererFeature s_Instance;
        readonly DecalForwardOpaquePass m_ForwardOpaquePass;
        readonly DecalForwardTransparentPass m_ForwardTransparentPass;
#endregion

#region Constructors
        public KDecalRendererFeature()
        {
            s_Instance = this;
            m_ForwardOpaquePass = new DecalForwardOpaquePass();
            m_ForwardTransparentPass = new DecalForwardTransparentPass();
        }
#endregion

#region Initialization
        public override void Create()
        {
            name = "kDecals";
        }
#endregion
        
#region RenderPass
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            // Enqueue passes
            renderer.EnqueuePass(m_ForwardOpaquePass);
            renderer.EnqueuePass(m_ForwardTransparentPass);
        }
#endregion
    }
}
