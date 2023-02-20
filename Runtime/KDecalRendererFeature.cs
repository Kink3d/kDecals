using UnityEngine.Rendering.Universal;

namespace kTools.Decals
{
    [DisallowMultipleRendererFeature("kDecals")]
    sealed class KDecalRendererFeature : ScriptableRendererFeature
    {
#region Fields
        static KDecalRendererFeature s_Instance;
        readonly DecalRenderPass m_RenderPass;
#endregion

#region Constructors
        public KDecalRendererFeature()
        {
            s_Instance = this;
            m_RenderPass = new DecalRenderPass();
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
            renderer.EnqueuePass(m_RenderPass);
        }
#endregion
    }
}
