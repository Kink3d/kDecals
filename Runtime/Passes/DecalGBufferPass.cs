using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;

namespace kTools.Decals
{
    sealed class DecalGBufferPass : DecalPass
    {
        public override string passName => "Decal GBuffer";
        public override string passTag => "DecalGBuffer";

        private UniversalRenderer m_Renderer;
        private DeferredLights m_DeferredLights;

        public DecalGBufferPass()
        {
            // Set data
            renderPassEvent = RenderPassEvent.AfterRenderingGbuffer;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            m_Renderer = renderingData.cameraData.renderer as UniversalRenderer;
            m_DeferredLights = m_Renderer.deferredLights;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            // We must explicitely specify we don't want any clear to avoid unwanted side-effects.
            // ScriptableRenderer will implicitely force a clear the first time the camera color/depth targets are bound.
            ConfigureTarget(m_DeferredLights.GbufferAttachmentIdentifiers, m_DeferredLights.DepthAttachmentIdentifier, m_DeferredLights.GbufferFormats);
            ConfigureClear(ClearFlag.None, Color.black);
        }

        public override void FilterDecals(ref List<Decal> decals)
        {
            decals = DecalSystem.decals
                .Where(x => x.decalData? (!x.decalData.isTransparent && x.decalData.supportsDeferred) : false)
                .OrderBy(x => x.decalData? x.decalData.sortingOrder : 0)
                .ToList();
        }
    }
}
