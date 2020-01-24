using UnityEngine.Rendering.Universal;

namespace kTools.Decals
{
    sealed class DecalRendererFeature : ScriptableRendererFeature
    {
#region Fields
        static DecalRendererFeature s_Instance;
        readonly DecalRenderPass m_RenderPass;
#endregion

#region Constructors
        public DecalRendererFeature()
        {
            s_Instance = this;
            m_RenderPass = new DecalRenderPass();
        }
#endregion

#region Initialization
        public override void Create()
        {
            name = "Decals";
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
