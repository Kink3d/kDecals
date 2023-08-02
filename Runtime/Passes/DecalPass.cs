using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
        
        public abstract string passName { get; }

        public abstract void FilterDecals(ref List<Decal> decals);
        
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

                    // Render
                    RenderDecal(context, decal, cullingResults, ref renderingData);
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
            
            // Material
            drawingSettings.overrideMaterial = decal.decalData.material;
            drawingSettings.overrideMaterialPassIndex = 0;
            return drawingSettings;
        }
        
        void ExecuteCommand(ScriptableRenderContext context, CommandBuffer cmd)
        {
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
        }
    }
}
