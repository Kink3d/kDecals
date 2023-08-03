using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace kTools.Decals
{
    sealed class DecalGBufferPass : DecalPass
    {
        public override string passName => "Decal GBuffer";
        public override string passTag => "DecalGBuffer";

        private UniversalRenderer m_Renderer;
        private RenderTargetIdentifier m_DepthIdentifier;

        public DecalGBufferPass() : base()
        {
            renderPassEvent = RenderPassEvent.AfterRenderingGbuffer;
        }

        public bool enablePerChannelDecals { get; set; }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            base.OnCameraSetup(cmd, ref renderingData);

            SetupGBufferResources(renderingData);
            SetupGBufferCopyResources(renderingData, cmd, false);

            // For some reason cameraDepthIdentifier is unbound now
            // Just bind it manually here
            var depth = new RenderTargetHandle();
            depth.Init("_CameraDepthAttachment");
            m_DepthIdentifier = depth.Identifier();
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            // We must explicitely specify we don't want any clear to avoid unwanted side-effects.
            // ScriptableRenderer will implicitely force a clear the first time the camera color/depth targets are bound.
            ConfigureTarget(gbufferAttachmentIdentifiers, m_DepthIdentifier);
            ConfigureGBufferFormats();
            ConfigureClear(ClearFlag.None, Color.black);
        }

        public override void OnAfterRenderDecal(ScriptableRenderContext context, Decal decal, CommandBuffer cmd, ref RenderingData renderingData)
        {
            if(settings.gBufferUpdateFrequency == UpdateFrequency.Never ||
                settings.gBufferUpdateFrequency == UpdateFrequency.PerDecal && !decal.decalData.updateGBuffers)
                return;

            var length = gBufferCopyAttachments.Length;
            for(int i = 0; i < length; i++)
            {
                var identifier = gBufferCopyAttachments[i].Identifier();
                cmd.SetGlobalTexture("_SourceTex", gbufferAttachmentIdentifiers[i]);
                cmd.Blit(gbufferAttachmentIdentifiers[i], identifier, blitMaterial, 0);
            }

            cmd.SetRenderTarget(gbufferAttachmentIdentifiers, depthAttachment);
        }
        
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            CleanupRenderTextures(cmd);
        }

        public override void FilterDecals(ref List<Decal> decals)
        {
            decals = DecalSystem.decals
                .Where(x => x.decalData? (!x.decalData.isTransparent && x.decalData.supportsDeferred && !x.decalData.forceForward) : false)
                .OrderBy(x => x.decalData? x.decalData.sortingOrder : 0)
                .ToList();
        }
    }
}
