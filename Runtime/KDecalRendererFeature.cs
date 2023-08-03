using System.Reflection;
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

            // Calculate Rendering Mode
            var field = typeof(UniversalRenderer).GetField("m_RenderingMode", BindingFlags.Instance | BindingFlags.NonPublic);
            var renderingMode = (RenderingMode)field.GetValue(universalRenderer);

            // Enqueue passes
            if(renderingMode == RenderingMode.Deferred)
            {
                renderer.EnqueuePass(m_GBufferPass);
            }
            
            m_ForwardOpaquePass.renderingMode = renderingMode;
            renderer.EnqueuePass(m_ForwardOpaquePass);
            renderer.EnqueuePass(m_ForwardTransparentPass);
        }
#endregion
    }
}
