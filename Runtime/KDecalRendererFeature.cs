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
        
        public KDecalRendererFeature()
        {
            s_Instance = this;
            m_ForwardOpaquePass = new DecalForwardOpaquePass();
            m_ForwardTransparentPass = new DecalForwardTransparentPass();
            m_GBufferCopyPass = new DecalGBufferCopyPass();
            m_GBufferPass = new DecalGBufferPass();
        }
        
        public override void Create()
        {
            name = "kDecals";
        }
        
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            var universalRenderer = renderer as UniversalRenderer;
            if (universalRenderer == null)
                return;

            // Calculate Rendering Mode
            var field = typeof(UniversalRenderer).GetField("m_RenderingMode", BindingFlags.Instance | BindingFlags.NonPublic);
            var renderingMode = (RenderingMode)field.GetValue(universalRenderer);

            // Set pass flags
            var settings = DecalSettings.GetOrCreateSettings();
            m_GBufferPass.enablePerChannelDecals = settings.enablePerChannelDecals;
            m_ForwardOpaquePass.renderingMode = renderingMode;

            // Enqueue passes
            if(renderingMode == RenderingMode.Deferred)
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
