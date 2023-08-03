using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace kTools.Decals
{
    abstract class DecalPass : ScriptableRenderPass
    {
        const float kErrorMargin = 0.1f;

        static readonly string[] s_ShaderTags = new string[]
        {
            "UniversalForward",
            "LightweightForward",
            "SRPDefaultUnlit",
        };

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

        private int GBufferAlbedoIndex => 0;
        private int GBufferSpecularMetallicIndex => 1;
        private int GBufferNormalSmoothnessIndex => 2;
        private int GBufferLightingIndex => 3;

        private UniversalRenderer m_Renderer;
        private DecalSettings m_Settings;
        private RenderTargetIdentifier m_ColorAttachment;
        private RenderTargetHandle[] m_GBufferAttachments;
        private RenderTargetHandle[] m_GBufferCopyAttachments;
        private RenderTargetIdentifier[] m_GbufferAttachmentIdentifiers;
        private GraphicsFormat[] m_GBufferFormats;
        private Material m_BlitMaterial;

        public DecalPass()
        {
            m_ColorAttachment = colorAttachment;
        }
        
        public abstract string passName { get; }
        public abstract string passTag { get; }

        public UniversalRenderer renderer => m_Renderer;
        public DecalSettings settings => m_Settings;
        public RenderTargetIdentifier originalColorAttachment => m_ColorAttachment;
        public RenderTargetHandle[] gBufferAttachments => m_GBufferAttachments;
        public RenderTargetHandle[] gBufferCopyAttachments => m_GBufferCopyAttachments;
        public RenderTargetIdentifier[] gbufferAttachmentIdentifiers => m_GbufferAttachmentIdentifiers;
        public GraphicsFormat[] gBufferFormats => m_GBufferFormats;
        public Material blitMaterial => m_BlitMaterial;


        public abstract void FilterDecals(ref List<Decal> decals);
        public virtual void OnAfterRenderDecal(ScriptableRenderContext context, Decal decal, CommandBuffer command, ref RenderingData renderingData) {}

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            m_Settings = Resources.Load<DecalSettings>("DecalSettings");
            m_Renderer = renderingData.cameraData.renderer as UniversalRenderer;
            if(m_BlitMaterial == null)
            {
                var field = typeof(UniversalRenderer).GetField("m_BlitMaterial", BindingFlags.Instance | BindingFlags.NonPublic);
                m_BlitMaterial = (Material)field.GetValue(m_Renderer);
            }
        }
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // Profiling command
            CommandBuffer cmd = CommandBufferPool.Get(passName);
            using (new ProfilingSample(cmd, passName))
            {
                ExecuteCommand(context, cmd);

                // Sorting
                var decals = ListPool<Decal>.Get();
                FilterDecals(ref decals);

                foreach(var decal in decals)
                {
                    if(decal.decalData == null)
                        return;

                    // Culling
                    var cullingResults = new CullingResults();
                    if(!Culling(context, decal, ref renderingData, out cullingResults))
                        continue;

                    // Shader Uniforms
                    SetShaderUniforms(context, decal, cmd);
                    SetShaderKeywords(context, decal, cmd);

                    // Render
                    RenderDecal(context, decal, cullingResults, ref renderingData);
                    OnAfterRenderDecal(context, decal, cmd, ref renderingData);
                }

                ListPool<Decal>.Release(decals);
            }

            ExecuteCommand(context, cmd);
        }
        
        bool Culling(ScriptableRenderContext context, Decal decal, ref RenderingData renderingData, out CullingResults cullingResults)
        {
            // Setup
            var camera = renderingData.cameraData.camera;
            var localScale = decal.transform.lossyScale;
            cullingResults = new CullingResults();

            // Never draw in Preview
            if(camera.cameraType == CameraType.Preview)
                return false;

            // Test for Decal behind Camera
            var maxRadius = Mathf.Max(Mathf.Max(localScale.x * 0.5f, localScale.y * 0.5f), localScale.z) + kErrorMargin;
            var positionVS = camera.WorldToViewportPoint(decal.transform.position);
            if(positionVS.z < -maxRadius)
                return false;
            
            // Get Decal bounds
            var boundsScale = new Vector3(maxRadius, maxRadius, maxRadius);
            var bounds = new Bounds(decal.transform.position, boundsScale);
            
            // Test against frustum planes
            var planes = GeometryUtility.CalculateFrustumPlanes(camera);
            if(!GeometryUtility.TestPlanesAABB(planes, bounds))
                return false;
            
            // Get CullingParameters
            var cullingParameters = new ScriptableCullingParameters();
            if (!camera.TryGetCullingParameters(out cullingParameters))
                return false;

            // Set culling planes
            cullingParameters.cullingPlaneCount = 6;
            for (int i = 0; i < 6; ++i)
            {
                cullingParameters.SetCullingPlane(i, decal.clipPlanes[i]);
            }

            // Culling Results
            cullingResults = context.Cull(ref cullingParameters);
            return true;
        }
        
        void SetShaderUniforms(ScriptableRenderContext context, Decal decal, CommandBuffer cmd)
        {
            // Set Shader globals
            cmd.SetGlobalMatrix("decal_Projection", decal.matrix);
            cmd.SetGlobalVector("decal_Direction", decal.transform.forward);
            cmd.SetGlobalFloat("decal_DepthFalloff", decal.decalData.depthFalloff);
            cmd.SetGlobalFloat("decal_Angle", Mathf.Deg2Rad * (180 - decal.decalData.angle));
            cmd.SetGlobalFloat("decal_AngleFalloff", decal.decalData.angleFalloff);

            // Encode deferred flags
            cmd.SetGlobalFloat("decal_DeferredFlags", 
                (decal.decalData.affectAlbedo ? 1 : 0) |
                (decal.decalData.affectSpecular ? 2 : 0) |
                (decal.decalData.affectSmoothness ? 4 : 0) |
                (decal.decalData.affectNormal ? 8 : 0) |
                (decal.decalData.affectOcclusion ? 16 : 0));

            ExecuteCommand(context, cmd);
        }

        void SetShaderKeywords(ScriptableRenderContext context, Decal decal, CommandBuffer cmd)
        {
            var settings = DecalSettings.GetOrCreateSettings();
            SetKeyword(cmd, "_DECAL_PER_CHANNEL", settings.enablePerChannelDecals);

            ExecuteCommand(context, cmd);
        }
        
        void RenderDecal(ScriptableRenderContext context, Decal decal, CullingResults cullingResults, ref RenderingData renderingData)
        {
            // Create Settings
            var drawingSettings = GetDrawingSettings(decal, ref renderingData);
            var filteringSettings = new FilteringSettings(RenderQueueRange.all, decal.decalData.layerMask);
            var renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
            
            // Draw Renderers
			context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings, ref renderStateBlock);
        }

        DrawingSettings GetDrawingSettings(Decal decal, ref RenderingData renderingData)
        {
            // Drawing Settings
            var camera = renderingData.cameraData.camera;
            var sortingSettings = new SortingSettings(camera) { criteria = SortingCriteria.CommonOpaque };
            var drawingSettings = new DrawingSettings(ShaderTagId.none, sortingSettings)
            {
                perObjectData = renderingData.perObjectData,
                mainLightIndex = renderingData.lightData.mainLightIndex,
                enableDynamicBatching = renderingData.supportsDynamicBatching,
                enableInstancing = true,
            };

            // Shader Tags
            for (int i = 0; i < s_ShaderTags.Length; ++i)
            {
                drawingSettings.SetShaderPassName(i, new ShaderTagId(s_ShaderTags[i]));
            }

            // Get Material data
            var material = decal.decalData.material;
            var passIndex = 0;
            var passCount = material.passCount;
            for(int i = 0; i < passCount; i++)
            {
                var tagValue = material.shader.FindPassTagValue(i, new ShaderTagId("LightMode"));
                if(tagValue.name == passTag)
                {
                    passIndex = i;
                    break;
                }
            }
            
            // Override Material data
            drawingSettings.overrideMaterial = material;
            drawingSettings.overrideMaterialPassIndex = passIndex;
            return drawingSettings;
        }

        public void CleanupRenderTextures(CommandBuffer cmd)
        {
            if(m_GBufferCopyAttachments == null)
                return;

            // Release GBuffer Copies if they exist
            var length = m_GBufferCopyAttachments.Length;
            for(int i = 0; i < length; i++)
            {
                if (m_GBufferCopyAttachments[i] != RenderTargetHandle.CameraTarget)
                {
                    cmd.ReleaseTemporaryRT(m_GBufferCopyAttachments[i].id);
                    m_GBufferCopyAttachments[i] = RenderTargetHandle.CameraTarget;
                }
            }
        }

        // We cant access DeferredLights so we need to manage all our own identifiers...
        internal void SetupGBufferResources(RenderingData renderingData)
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

        internal void SetupGBufferCopyResources(RenderingData renderingData, CommandBuffer cmd, bool createTextures)
        {
            if(m_Settings == null || !m_Settings.enablePerChannelDecals)
                return;

            var length = m_GBufferAttachments.Length;
            m_GBufferCopyAttachments = new RenderTargetHandle[length];
            for(int i = 0; i < length; i++)
            {
                m_GBufferCopyAttachments[i] = new RenderTargetHandle();
                m_GBufferCopyAttachments[i].Init($"_GBuffer{i}Copy");

                if(createTextures)
                {
                    var descriptor = renderingData.cameraData.cameraTargetDescriptor;
                    descriptor.graphicsFormat = m_GBufferFormats[i];
                    cmd.GetTemporaryRT(m_GBufferCopyAttachments[i].id, descriptor, FilterMode.Point);
                }
            }
        }

        // The overload of ConfigureTarget that takes a GraphicsFormat array is internal
        // Of course the field it sets is also internal so we need to use reflection...
        internal void ConfigureGBufferFormats()
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
        internal GraphicsFormat GetGBufferFormat(int index)
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
        
        void ExecuteCommand(ScriptableRenderContext context, CommandBuffer cmd)
        {
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
        }

        void SetKeyword(CommandBuffer cmd, string keyword, bool value)
        {
            if(value)
            {
                cmd.EnableShaderKeyword(keyword);
            }
            else
            {
                cmd.DisableShaderKeyword(keyword);
            }
        }
    }
}
