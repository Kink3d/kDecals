using UnityEngine;
using UnityEditor;

namespace kTools.Decals.Editor
{
    using Editor = UnityEditor.Editor;

    [CustomEditor(typeof(Decal)), CanEditMultipleObjects]
    sealed class DecalEditor : Editor
    {
#region Structs
        struct Styles
        {
            // Properties
            public static readonly GUIContent Type = new GUIContent("Type",
                "Type of this Decal, used to determine rendering mode.");
            public static readonly GUIContent Data = new GUIContent("Data",
                "DecalData defining options and inputs for this Decal.");
            public static readonly GUIContent Mesh = new GUIContent("Mesh",
                "Mesh to use for rendering.");
            public static readonly GUIContent SubmeshIndex = new GUIContent("Submesh Index",
                "Index of submesh to use for rendering.");
        }

        struct PropertyNames
        {
            public static readonly string Type = "m_DecalType";
            public static readonly string DecalData = "m_DecalData";
            public static readonly string Mesh = "m_Mesh";
            public static readonly string SubmeshIndex = "m_SubmeshIndex";
        }
#endregion

#region Fields
        Decal m_Target;

        // Properties
        SerializedProperty m_DecalTypeProp;
        SerializedProperty m_DecalDataProp;
        SerializedProperty m_MeshProp;
        SerializedProperty m_SubmeshIndexProp;
#endregion

#region State
        void OnEnable()
        {
            // Set data
            m_Target = target as Decal;

            // Get Properties
            m_DecalTypeProp = serializedObject.FindProperty(PropertyNames.Type);
            m_DecalDataProp = serializedObject.FindProperty(PropertyNames.DecalData);
            m_MeshProp = serializedObject.FindProperty(PropertyNames.Mesh);
            m_SubmeshIndexProp = serializedObject.FindProperty(PropertyNames.SubmeshIndex);
        }
#endregion

#region GUI
        public override void OnInspectorGUI()
        {
            // Setup
            serializedObject.Update();

            // Data
            EditorGUILayout.PropertyField(m_DecalDataProp, Styles.Data);
            EditorGUILayout.PropertyField(m_DecalTypeProp, Styles.Type);

            if((DecalType)m_DecalTypeProp.intValue == DecalType.Mesh)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_MeshProp, Styles.Mesh);
                EditorGUILayout.PropertyField(m_SubmeshIndexProp, Styles.SubmeshIndex);
                EditorGUI.indentLevel--;
            }

            // Finalize
            serializedObject.ApplyModifiedProperties();
        }
#endregion
    }
}
