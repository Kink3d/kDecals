using UnityEngine;
using UnityEditor;

namespace kTools.Decals.Editor
{
    sealed class LitGUI : BaseGUI
    {
#region Structs
        struct Labels
        {
            public static readonly GUIContent Color = new GUIContent("Color",
                "Specifies the base map and color of the surface. Alpha values are used for transparency.");

            public static readonly GUIContent Specular = new GUIContent("Specular", 
                "Sets and configures the map and color for the Specular workflow.");

            public static readonly GUIContent Metallic = new GUIContent("Metallic", 
                "Sets and configures the map for the Metallic workflow.");

            public static readonly GUIContent Smoothness = new GUIContent("Smoothness",
                "Controls the spread of highlights and reflections on the surface.");

            public static readonly GUIContent Normal = new GUIContent("Normal", 
                "Assigns a tangent-space normal map.");

            public static readonly GUIContent Occlusion = new GUIContent("Occlusion",
                "Sets an occlusion map to simulate shadowing from ambient lighting.");

            public static readonly GUIContent Emission = new GUIContent("Emission",
                "Sets a map and color to use for emission. Colors are multiplied over the Texture.");
        }

        struct PropertyNames
        {
            public static readonly string BaseTex = "_BaseTex";
            public static readonly string Color = "_Color";
            public static readonly string Metallic = "_Metallic";
            public static readonly string SpecColor = "_SpecColor";
            public static readonly string MetallicGlossTex = "_MetallicGlossTex";
            public static readonly string SpecGlossTex = "_SpecGlossTex";
            public static readonly string Glossiness = "_Glossiness";
            public static readonly string BumpTex = "_BumpTex";
            public static readonly string BumpScale = "_BumpScale";
            public static readonly string OcclusionTex = "_OcclusionTex";
            public static readonly string OcclusionStrength = "_OcclusionStrength";
            public static readonly string EmissionTex = "_EmissionTex";
            public static readonly string EmissionColor = "_EmissionColor";
        }
#endregion

#region Fields
        MaterialProperty m_BaseTexProp;
        MaterialProperty m_ColorProp;
        MaterialProperty m_MetallicProp;
        MaterialProperty m_SpecColorProp;
        MaterialProperty m_MetallicGlossTexProp;
        MaterialProperty m_SpecGlossTexProp;
        MaterialProperty m_GlossinessProp;
        MaterialProperty m_BumpTexProp;
        MaterialProperty m_BumpScaleProp;
        MaterialProperty m_OcclusionTexProp;
        MaterialProperty m_OcclusionStrengthProp;
        MaterialProperty m_EmissionTexProp;
        MaterialProperty m_EmissionColorProp;
#endregion

#region GUI
        public override void GetProperties(MaterialProperty[] properties)
        {
            // Find properties
            m_BaseTexProp = FindProperty(PropertyNames.BaseTex, properties, false);
            m_ColorProp = FindProperty(PropertyNames.Color, properties, false);
            m_MetallicProp = FindProperty(PropertyNames.Metallic, properties);
            m_SpecColorProp = FindProperty(PropertyNames.SpecColor, properties, false);
            m_MetallicGlossTexProp = FindProperty(PropertyNames.MetallicGlossTex, properties);
            m_SpecGlossTexProp = FindProperty(PropertyNames.SpecGlossTex, properties, false);
            m_GlossinessProp = FindProperty(PropertyNames.Glossiness, properties, false);
            m_BumpTexProp = FindProperty(PropertyNames.BumpTex, properties, false);
            m_BumpScaleProp = FindProperty(PropertyNames.BumpScale, properties, false);
            m_OcclusionTexProp = FindProperty(PropertyNames.OcclusionTex, properties, false);
            m_OcclusionStrengthProp = FindProperty(PropertyNames.OcclusionStrength, properties, false);
            m_EmissionTexProp = FindProperty(PropertyNames.EmissionTex, properties, false);
            m_EmissionColorProp = FindProperty(PropertyNames.EmissionColor, properties, false);
        }

        public override void DrawSurfaceInputs(MaterialEditor materialEditor)
        {
            // Get Material
            var material = materialEditor.target as Material;

            // Color
            materialEditor.TexturePropertySingleLine(Labels.Color, m_BaseTexProp, m_ColorProp);

            // MetallicSpecular
            bool hasGlossMap = false;
            if (material.IsSpecularWorkflow())
            {
                hasGlossMap = m_SpecGlossTexProp.textureValue != null;
                materialEditor.TexturePropertySingleLine(Labels.Specular, m_SpecGlossTexProp, hasGlossMap ? null : m_SpecColorProp);
            }
            else
            {
                hasGlossMap = m_MetallicGlossTexProp.textureValue != null;
                materialEditor.TexturePropertySingleLine(Labels.Metallic, m_MetallicGlossTexProp, hasGlossMap ? null : m_MetallicProp);
            }

            // Smoothness
            EditorGUI.BeginChangeCheck();
            EditorGUI.indentLevel += 2;
            var smoothness = EditorGUILayout.Slider(Labels.Smoothness, m_GlossinessProp.floatValue, 0f, 1f);
            EditorGUI.indentLevel -= 2;
            if (EditorGUI.EndChangeCheck())
            {
                m_GlossinessProp.floatValue = smoothness;
            }

            // Normal
            materialEditor.TexturePropertySingleLine(Labels.Normal, m_BumpTexProp, m_BumpScaleProp);

            // Occlusion
            materialEditor.TexturePropertySingleLine(Labels.Occlusion, m_OcclusionTexProp, 
                m_OcclusionTexProp.textureValue != null ? m_OcclusionStrengthProp : null);

            // Emission
            var hadEmissionTexture = m_EmissionTexProp.textureValue != null;
            materialEditor.TexturePropertyWithHDRColor(Labels.Emission, m_EmissionTexProp,
                m_EmissionColorProp, false);

            // If texture was assigned and color was black set color to white
            var brightness = m_EmissionColorProp.colorValue.maxColorComponent;
            if (m_EmissionTexProp.textureValue != null && !hadEmissionTexture && brightness <= 0f)
                m_EmissionColorProp.colorValue = Color.white;
        }
#endregion

#region Keywords
        public override void SetMaterialKeywords(Material material)
        {
            // Metallic Specular
            var isSpecularWorkFlow = (WorkflowMode) material.GetFloat("_WorkflowMode") == WorkflowMode.Specular;
            var hasGlossMap = false;
            if (isSpecularWorkFlow)
                hasGlossMap = material.GetTexture(PropertyNames.SpecGlossTex) != null;
            else
                hasGlossMap = material.GetTexture(PropertyNames.MetallicGlossTex) != null;
            material.SetKeyword("_METALLICSPECGLOSSMAP", hasGlossMap);

            // Normal
            material.SetKeyword("_NORMALMAP", material.GetTexture(PropertyNames.BumpTex) != null);
        }
#endregion
    }
}
