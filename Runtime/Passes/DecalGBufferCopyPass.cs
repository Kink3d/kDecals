using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace kTools.Decals
{
    sealed class DecalGBufferCopyPass : DecalPass
    {
        public override string passName => "Decal GBuffer Copy";
        public override string passTag => "";
        public override void FilterDecals(ref List<Decal> decals) {}

        private UniversalRenderer m_Renderer;

        public DecalGBufferCopyPass() : base()
        {
            renderPassEvent = RenderPassEvent.AfterRenderingGbuffer;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            base.OnCameraSetup(cmd, ref renderingData);

            SetupGBufferResources(renderingData);
            SetupGBufferCopyResources(renderingData, cmd, true);
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            ConfigureGBufferFormats();
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if(settings != null && settings.enablePerChannelDecals)
            {
                var cmd = CommandBufferPool.Get("Copy GBuffers");
                using (new ProfilingSample(cmd, "Copy GBuffers"))
                {
                    // Flush
                    context.ExecuteCommandBuffer(cmd);
                    cmd.Clear();

                    var length = gBufferCopyAttachments.Length;
                    for(int i = 0; i < length; i++)
                    {
                        var identifier = gBufferCopyAttachments[i].Identifier();
                        cmd.SetGlobalTexture("_SourceTex", gbufferAttachmentIdentifiers[i]);
                        cmd.Blit(gbufferAttachmentIdentifiers[i], identifier, blitMaterial, 0);
                    }
                }

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
        }
    }
}
