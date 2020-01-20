using UnityEngine;
using UnityEditor;

namespace kTools.Decals.Editor
{
    using Editor = UnityEditor.Editor;

    [CustomEditor(typeof(Decal))]
    sealed class DecalEditor : Editor
    {
#region Structs
        struct Styles
        {
            // Properties
            public static readonly GUIContent Data = new GUIContent("Data",
                "DecalData defining options and inputs for this Decal.");
        }

        struct PropertyNames
        {
            public static readonly string DecalData = "m_DecalData";
        }
#endregion

#region Fields
        Decal m_Target;

        // Properties
        SerializedProperty m_DecalDataProp;
#endregion

#region State
        void OnEnable()
        {
            // Set data
            m_Target = target as Decal;

            // Get Properties
            m_DecalDataProp = serializedObject.FindProperty(PropertyNames.DecalData);
        }
#endregion

#region GUI
        public override void OnInspectorGUI()
        {
            // Setup
            serializedObject.Update();

            // Data
            EditorGUILayout.PropertyField(m_DecalDataProp, Styles.Data);

            // Finalize
            serializedObject.ApplyModifiedProperties();
        }
#endregion
    }
}
