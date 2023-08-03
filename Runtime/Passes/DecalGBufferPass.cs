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
        static readonly string[] kGBufferNames = new string[]
        {
            "_GBuffer0",
            "_GBuffer1",
            "_GBuffer2",
            "_GBuffer3",
            "_GBuffer4",
            "_GBuffer5",
            "_GBuffer6"
        };

        public override string passName => "Decal GBuffer";
        public override string passTag => "DecalGBuffer";

        private int GBufferAlbedoIndex => 0;
        private int GBufferSpecularMetallicIndex => 1;
        private int GBufferNormalSmoothnessIndex => 2;
        private int GBufferLightingIndex => 3;

        private UniversalRenderer m_Renderer;
        private RenderTargetIdentifier m_ColorAttachment;
        private RenderTargetHandle[] m_GBufferAttachments;
        private RenderTargetIdentifier[] m_GbufferAttachmentIdentifiers;
        private GraphicsFormat[] m_GBufferFormats;

        public DecalGBufferPass()
        {
            // Set data
            renderPassEvent = RenderPassEvent.AfterRenderingGbuffer;
            m_ColorAttachment = colorAttachment;
        }

        public bool enablePerChannelDecals { get; set; }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            m_Renderer = renderingData.cameraData.renderer as UniversalRenderer;
            SetupGBufferResources(renderingData);
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            // We must explicitely specify we don't want any clear to avoid unwanted side-effects.
            // ScriptableRenderer will implicitely force a clear the first time the camera color/depth targets are bound.
            ConfigureTarget(m_GbufferAttachmentIdentifiers, depthAttachment);
            ConfigureGBufferFormats();
            ConfigureClear(ClearFlag.None, Color.black);
        }

        public override void FilterDecals(ref List<Decal> decals)
        {
            decals = DecalSystem.decals
                .Where(x => x.decalData? (!x.decalData.isTransparent && x.decalData.supportsDeferred) : false)
                .OrderBy(x => x.decalData? x.decalData.sortingOrder : 0)
                .ToList();
        }

        // We cant access DeferredLights so we need to manage all our own identifiers...
        private void SetupGBufferResources(RenderingData renderingData)
        {
            var gbufferSliceCount = 4;
            m_GBufferAttachments = new RenderTargetHandle[gbufferSliceCount];
            m_GbufferAttachmentIdentifiers = new RenderTargetIdentifier[gbufferSliceCount];
            m_GBufferFormats = new GraphicsFormat[gbufferSliceCount];

            for (int i = 0; i < gbufferSliceCount; i++)
            {
                m_GBufferAttachments[i].Init(kGBufferNames[i]);
                m_GbufferAttachmentIdentifiers[i] = m_GBufferAttachments[i].Identifier();
                m_GBufferFormats[i] = GetGBufferFormat(i);
            }

            m_GbufferAttachmentIdentifiers[GBufferLightingIndex] = m_ColorAttachment;

            #if ENABLE_VR && ENABLE_XR_MODULE
            // In XR SinglePassInstance mode, the RTs are texture-array and all slices must be bound.
            if (renderingData.cameraData.xr.enabled)
            {
                for (int i = 0; i < gbufferSliceCount; i++)
                {
                    m_GbufferAttachmentIdentifiers[i] = new RenderTargetIdentifier(m_GbufferAttachmentIdentifiers[i], 0, CubemapFace.Unknown, -1);
                }
            }
            #endif
        }

        // The overload of ConfigureTarget that takes a GraphicsFormat array is internal
        // Of course the field it sets is also internal so we need to use reflection...
        private void ConfigureGBufferFormats()
        {
            var property = typeof(ScriptableRenderPass).GetProperty("renderTargetFormat", BindingFlags.Instance | BindingFlags.NonPublic);
            var formats = (GraphicsFormat[])property.GetValue(this);
            for(int i = 0; i < m_GBufferFormats.Length; i++)
            {
                formats[i] = m_GBufferFormats[i];
            }
            property.SetValue(this, formats);
        }

        // Copy of DeferredLights.GetGBufferFormat since its internal
        // We omit RenderPass, Shadowmask and RenderingLayers
        private GraphicsFormat GetGBufferFormat(int index)
        {
            if (index == GBufferAlbedoIndex) // sRGB albedo, materialFlags
                return QualitySettings.activeColorSpace == ColorSpace.Linear ? GraphicsFormat.R8G8B8A8_SRGB : GraphicsFormat.R8G8B8A8_UNorm;
            else if (index == GBufferSpecularMetallicIndex) // sRGB specular, [unused]
                return GraphicsFormat.R8G8B8A8_UNorm;
            // TODO: Do we need to support AccurateGbufferNormals?
            else if (index == GBufferNormalSmoothnessIndex)
                return /*this.AccurateGbufferNormals ? GraphicsFormat.R8G8B8A8_UNorm : */GraphicsFormat.R8G8B8A8_SNorm; // normal normal normal packedSmoothness
            else if (index == GBufferLightingIndex) // Emissive+baked: Most likely B10G11R11_UFloatPack32 or R16G16B16A16_SFloat
                return GraphicsFormat.None;
            else
                return GraphicsFormat.None;
        }
    }
}
