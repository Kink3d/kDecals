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
            public static readonly string BaseTex = "_BaseTex";
            public static readonly string Color = "_Color";
        }
#endregion

#region Fields
        MaterialProperty m_BaseTexProp;
        MaterialProperty m_ColorProp;
#endregion

#region GUI
        public override void GetProperties(MaterialProperty[] properties)
        {
            // Find properties
            m_BaseTexProp = FindProperty(PropertyNames.BaseTex, properties, false);
            m_ColorProp = FindProperty(PropertyNames.Color, properties, false);
        }

        public override void DrawSurfaceInputs(MaterialEditor materialEditor)
        {
            // Color
            materialEditor.TexturePropertySingleLine(Labels.Color, m_BaseTexProp, m_ColorProp);
        }
#endregion
    }
}
