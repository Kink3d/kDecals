﻿using System;
using UnityEngine;
using UnityEditor;

namespace kTools.Decals.Editor
{
    /// <summary>
    /// Base ShaderGUI class for Decal Shaders.
    /// </summary>
    public abstract class BaseGUI : ShaderGUI
    {
#region Structs
        struct Styles
        {
            // Foldouts
            public static readonly GUIContent SurfaceOptions = new GUIContent("Surface Options");
            public static readonly GUIContent SurfaceInputs = new GUIContent("Surface Inputs");
            public static readonly GUIContent AdvancedOptions = new GUIContent("Advanced Options");

            // Properies
            public static readonly GUIContent WorkflowMode = new GUIContent("Workflow Mode",
                "Select a workflow that fits your textures. Choose between Metallic or Specular.");

            public static readonly GUIContent BlendingMode = new GUIContent("Blending Mode",
                "Controls how the color of the Transparent surface blends with the Material color in the background.");

            public static readonly GUIContent AlphaClipping = new GUIContent("Alpha Clipping",
                "Makes your Material act like a Cutout shader. Use this to create a transparent effect with hard edges between opaque and transparent areas.");

            public static readonly GUIContent AlphaClippingThreshold = new GUIContent("Threshold",
                "Sets where the Alpha Clipping starts. The higher the value is, the brighter the  effect is when clipping starts.");

            public static readonly GUIContent SpecularHighlights = new GUIContent("Specular Highlights",
                "When enabled, the Material reflects the shine from direct lighting.");

            public static readonly GUIContent EnvironmentReflections = new GUIContent("Environment Reflections",
                "When enabled, the Material samples reflections from the nearest Reflection Probes or Lighting Probe.");
        }

        struct PropertyNames
        {
            public static readonly string WorkflowMode = "_WorkflowMode";
            public static readonly string Blend = "_Blend";
            public static readonly string AlphaClip = "_AlphaClip";
            public static readonly string Cutoff = "_Cutoff";
            public static readonly string SpecularHighlights = "_SpecularHighlights";
            public static readonly string EnvironmentReflections = "_GlossyReflections";
        }
#endregion

#region Enumerations
        /// <summary>
        /// Blend mode enumeration for shaders.
        /// </summary>
        public enum BlendMode
        {
            Alpha,
            Premultiply,
            Additive,
            Multiply,
        }

        /// <summary>
        /// Workflow mode (specular/metallic) enumeration for shaders.
        /// </summary>
        public enum WorkflowMode
        {
            Specular,
            Metallic,
        }
#endregion

#region Fields
        // Foldouts
        bool m_SurfaceOptionsFoldout;
        bool m_SurfaceInputsFoldout;
        bool m_AdvancedOptionsFoldout;

        // Properties
        MaterialProperty m_WorkflowModeProp;
        MaterialProperty m_BlendProp;
        MaterialProperty m_AlphaClipProp;
        MaterialProperty m_CutoffProp;
        MaterialProperty m_SpecularHighlightsProp;
        MaterialProperty m_EnvironmentReflectionsProp;
#endregion

#region Constructors
        public BaseGUI()
        {
            m_SurfaceOptionsFoldout = true;
            m_SurfaceInputsFoldout = true;
            m_AdvancedOptionsFoldout = true;
        }
#endregion

#region GUI
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            // Base properties
            m_WorkflowModeProp = FindProperty(PropertyNames.WorkflowMode, properties, false);
            m_BlendProp = FindProperty(PropertyNames.Blend, properties, false);
            m_AlphaClipProp = FindProperty(PropertyNames.AlphaClip, properties, false);
            m_CutoffProp = FindProperty(PropertyNames.Cutoff, properties, false);
            m_SpecularHighlightsProp = FindProperty(PropertyNames.SpecularHighlights, properties, false);
            m_EnvironmentReflectionsProp = FindProperty(PropertyNames.EnvironmentReflections, properties, false);

            // Leaf properties
            GetProperties(properties);

            // Draw properties
            EditorGUI.BeginChangeCheck();
            DrawProperties(materialEditor);
            if (EditorGUI.EndChangeCheck())
            {
                SetBaseMaterialKeywords(materialEditor.target as Material);
            }
        }
#endregion

#region Properties
        /// <summary>
        /// Get MaterialProperty fields during OnGUI call.
        /// </summary>
        /// <param name="properties">MaterialProperty array to access.</param>  
        public abstract void GetProperties(MaterialProperty[] properties);

        /// <summary>
        /// Draw MaterialProperty fields within the `Surface Inputs` foldout.
        /// </summary>
        /// <param name="materialEditor">MaterialEditor currently drawing.</param>  
        public abstract void DrawSurfaceInputs(MaterialEditor materialEditor);

        void DrawProperties(MaterialEditor materialEditor)
        {
            // Surface Options
            m_SurfaceOptionsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_SurfaceOptionsFoldout, Styles.SurfaceOptions);
            if(m_SurfaceOptionsFoldout)
            {
                DrawSurfaceProperies(materialEditor);
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            // Surface Inputs
            m_SurfaceInputsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_SurfaceInputsFoldout, Styles.SurfaceInputs);
            if(m_SurfaceInputsFoldout)
            {
                DrawSurfaceInputs(materialEditor);
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            // Advanced Options
            m_AdvancedOptionsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_AdvancedOptionsFoldout, Styles.AdvancedOptions);
            if(m_AdvancedOptionsFoldout)
            {
                DrawAdvancedOptions(materialEditor);
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        void DrawSurfaceProperies(MaterialEditor materialEditor)
        {
            // Get Material
            var material = materialEditor.target as Material;

            // Workflow Mode
            if(material.HasProperty(PropertyNames.WorkflowMode))
            {
                EditorGUI.BeginChangeCheck();
                var workflowMode = EditorGUILayout.Popup(Styles.WorkflowMode, (int)m_WorkflowModeProp.floatValue, Enum.GetNames(typeof(WorkflowMode)));
                if (EditorGUI.EndChangeCheck())
                {
                    materialEditor.RegisterPropertyChangeUndo(Styles.WorkflowMode.text);
                    m_WorkflowModeProp.floatValue = workflowMode;
                }
            }

            // Blend Mode
            if(material.HasProperty(PropertyNames.Blend))
            {
                EditorGUI.BeginChangeCheck();
                var blend = EditorGUILayout.Popup(Styles.BlendingMode, (int)m_BlendProp.floatValue, Enum.GetNames(typeof(BlendMode)));
                if (EditorGUI.EndChangeCheck())
                {
                    materialEditor.RegisterPropertyChangeUndo(Styles.BlendingMode.text);
                    m_BlendProp.floatValue = blend;
                }
            }

            // AlphaClip Enabled
            if(material.HasProperty(PropertyNames.AlphaClip) && material.HasProperty(PropertyNames.Cutoff))
            {
                EditorGUI.BeginChangeCheck();
                var alphaClip = EditorGUILayout.Toggle(Styles.AlphaClipping, m_AlphaClipProp.floatValue == 1);
                if (EditorGUI.EndChangeCheck())
                {
                    m_AlphaClipProp.floatValue = alphaClip ? 1 : 0;
                }

                // Alpha Clip
                if (m_AlphaClipProp.floatValue == 1)
                {
                    materialEditor.ShaderProperty(m_CutoffProp, Styles.AlphaClippingThreshold, 1);
                }
            }
        }

        void DrawAdvancedOptions(MaterialEditor materialEditor)
        {
            // Get Material
            var material = materialEditor.target as Material;

            // Highlights
            if(material.HasProperty(PropertyNames.SpecularHighlights))
            {
                materialEditor.ShaderProperty(m_SpecularHighlightsProp, Styles.SpecularHighlights);
            }

            // Reflections
            if(material.HasProperty(PropertyNames.EnvironmentReflections))
            {
                materialEditor.ShaderProperty(m_EnvironmentReflectionsProp, Styles.EnvironmentReflections);
            }
        }
#endregion

#region Keywords
        /// <summary>
        /// Set Material keywords when changes are made during OnGUI call.
        /// </summary>
        /// <param name="material">Material target of current MaterialEditor.</param>
        public virtual void SetMaterialKeywords(Material material) {}

        void SetBaseMaterialKeywords(Material material)
        {
            // Reset
            material.shaderKeywords = null;

            // Custom Keywords
            SetMaterialKeywords(material);

            // WorkflowMode
            if(material.HasProperty(PropertyNames.WorkflowMode))
            {
                material.SetKeyword("_SPECULAR_SETUP", material.IsSpecularWorkflow());
            }

            // BlendMode
            if(material.HasProperty(PropertyNames.Blend))
            {
                BlendMode blend = (BlendMode)material.GetFloat(m_BlendProp.name);
                switch (blend)
                {
                    case BlendMode.Alpha:
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        break;
                    case BlendMode.Premultiply:
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                        break;
                    case BlendMode.Additive:
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        break;
                    case BlendMode.Multiply:
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.DstColor);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.EnableKeyword("_ALPHAMODULATE_ON");
                        break;
                }
            }

            // AlphaClip
            if(material.HasProperty(PropertyNames.AlphaClip))
            {
                material.SetKeyword("_ALPHATEST_ON", material.GetFloat(m_AlphaClipProp.name) == 1);
            }
            
            // Highlights
            if(material.HasProperty(PropertyNames.SpecularHighlights))
            {
                material.SetKeyword("_SPECULARHIGHLIGHTS_OFF", material.GetFloat(m_SpecularHighlightsProp.name) == 0.0f);
            }

            // Reflections
            if(material.HasProperty(PropertyNames.EnvironmentReflections))
            {
                material.SetKeyword("_ENVIRONMENTREFLECTIONS_OFF", material.GetFloat(m_EnvironmentReflectionsProp.name) == 0.0f);
            }
        }        
#endregion
    }

    public static class ShaderGUIExtensions
    {
#region Workflow
        /// <summary>
        /// Determine if current material is using specular `WorkflowMode`.
        /// </summary>
        /// <returns>True if specular `WorkflowMode`. Default is false.</returns>
        public static bool IsSpecularWorkflow(this Material material)
        {
            if(!material.HasProperty("_WorkflowMode"))
                return false;
            
            return (BaseGUI.WorkflowMode)material.GetFloat("_WorkflowMode") == BaseGUI.WorkflowMode.Specular;
        }
#endregion

#region Keywords
        /// <summary>
        /// Sets `keyword` on current Material to `value`.
        /// </summary>
        /// <param name="keyword">Keyword string to set.</param>
        /// <param name="value">Value to set keyword.</param>
        public static void SetKeyword(this Material material, string keyword, bool value)
        {
            if (value)
            {
                material.EnableKeyword(keyword);
            }
            else
            {
                material.DisableKeyword(keyword);
            }
        }
#endregion
    }
}
