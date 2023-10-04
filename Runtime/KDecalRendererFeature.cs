using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace kTools.Decals
{
    [DisallowMultipleRendererFeature("kDecals")]
    internal sealed class KDecalRendererFeature : ScriptableRendererFeature
    {
        static KDecalRendererFeature s_Instance;
        readonly DecalForwardOpaquePass m_ForwardOpaquePass;
        readonly DecalForwardTransparentPass m_ForwardTransparentPass;
        readonly DecalGBufferCopyPass m_GBufferCopyPass;
        readonly DecalGBufferPass m_GBufferPass;
        readonly System.Reflection.FieldInfo m_RenderingModeInfo;
        private UniversalRenderer m_Renderer;
        
        public KDecalRendererFeature()
        {
            s_Instance = this;
            m_ForwardOpaquePass = new DecalForwardOpaquePass();
            m_ForwardTransparentPass = new DecalForwardTransparentPass();
            m_GBufferCopyPass = new DecalGBufferCopyPass();
            m_GBufferPass = new DecalGBufferPass();
            m_RenderingModeInfo = typeof(UniversalRenderer).GetField("m_RenderingMode", BindingFlags.Instance | BindingFlags.NonPublic);
        }
        
        public override void Create()
        {
            name = "kDecals";
        }

        protected override void Dispose(bool disposing)
        {
            m_Renderer = null;
            base.Dispose(disposing);
        }
        
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            var universalRenderer = renderer as UniversalRenderer;
            if (universalRenderer == null)
                return;

            if (m_Renderer != universalRenderer)
            {
                m_Renderer = universalRenderer;

                // Calculate Rendering Mode
                var renderingMode = (RenderingMode)m_RenderingModeInfo.GetValue(universalRenderer);
                m_ForwardOpaquePass.renderingMode = renderingMode;
            }

            // Set pass flags
            var settings = DecalSettings.GetOrCreateSettings();
            m_GBufferPass.enablePerChannelDecals = settings.enablePerChannelDecals;

            // Enqueue passes
            if(m_ForwardOpaquePass.renderingMode == RenderingMode.Deferred)
            {
                if(settings.enablePerChannelDecals)
                {
                    renderer.EnqueuePass(m_GBufferCopyPass);
                }

                renderer.EnqueuePass(m_GBufferPass);
            }
            
            renderer.EnqueuePass(m_ForwardOpaquePass);
            renderer.EnqueuePass(m_ForwardTransparentPass);
        }
    }
}
