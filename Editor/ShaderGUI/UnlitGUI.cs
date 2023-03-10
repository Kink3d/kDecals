using UnityEngine;
using UnityEditor;

namespace kTools.Decals.Editor
{
    sealed class UnlitGUI : BaseGUI
    {
#region Structs
        class Labels
        {
            public static readonly GUIContent Color = new GUIContent("Color",
                "Specifies the base map and color of the surface. Alpha values are used for transparency.");
        }

        class PropertyNames
        {
            public static readonly string BaseMap = "_BaseMap";
            public static readonly string BaseColor = "_BaseColor";
        }
#endregion

#region Fields
        MaterialProperty m_BaseMapProp;
        MaterialProperty m_BaseColorProp;
#endregion

#region GUI
        public override void GetProperties(MaterialProperty[] properties)
        {
            // Find properties
            m_BaseMapProp = FindProperty(PropertyNames.BaseMap, properties, false);
            m_BaseColorProp = FindProperty(PropertyNames.BaseColor, properties, false);
        }

        public override void DrawSurfaceInputs(MaterialEditor materialEditor)
        {
            // Color
            materialEditor.TexturePropertySingleLine(Labels.Color, m_BaseMapProp, m_BaseColorProp);

            // Scale & Offset
            materialEditor.TextureScaleOffsetProperty(m_BaseMapProp);
        }
#endregion
    }
}
