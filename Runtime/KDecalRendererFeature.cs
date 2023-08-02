using UnityEngine.Rendering;
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
        readonly DecalGBufferPass m_GBufferPass;
#endregion

#region Constructors
        public KDecalRendererFeature()
        {
            s_Instance = this;
            m_ForwardOpaquePass = new DecalForwardOpaquePass();
            m_ForwardTransparentPass = new DecalForwardTransparentPass();
            m_GBufferPass = new DecalGBufferPass();
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
            var universalRenderer = renderer as UniversalRenderer;
            if (universalRenderer == null)
                return;

            // Enqueue passes
            var forwardOnly = false;
            if(universalRenderer.renderingMode == RenderingMode.Deferred)
            {
                renderer.EnqueuePass(m_GBufferPass);
                forwardOnly = true;
            }
            
            m_ForwardOpaquePass.forwardOnly = forwardOnly;
            renderer.EnqueuePass(m_ForwardOpaquePass);
            renderer.EnqueuePass(m_ForwardTransparentPass);
        }
#endregion
    }
}
