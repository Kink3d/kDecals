using UnityEngine;
using UnityEditor;

namespace kTools.Decals.Editor
{
    using Editor = UnityEditor.Editor;

    [CustomEditor(typeof(DecalData))]
    sealed class DecalDataEditor : Editor
    {
#region Structs
        struct Styles
        {
            // Foldouts
            public static readonly GUIContent PoolingOptions = new GUIContent("Pooling Options");
            public static readonly GUIContent ProjectionOptions = new GUIContent("Projection Options");

            // Properties
            public static readonly GUIContent PoolingEnabled = new GUIContent("Enabled",
                "If enabled all Decals created via DecalSystem will be pre-generated and taken from a Pool.");

            public static readonly GUIContent InstanceCount = new GUIContent("Instance Count",
                "How many instances of this Decal will be created in the Pool.");

            public static readonly GUIContent Depth = new GUIContent("Depth",
                "How far the decal projection should blend on Z axis.");
        }

        struct PropertyNames
        {
            public static readonly string PoolingEnabled = "m_PoolingEnabled";
            public static readonly string InstanceCount = "m_InstanceCount";
            public static readonly string ProjectionDepth = "m_ProjectionDepth";
        }
#endregion

#region Fields
        DecalData m_Target;
        MaterialEditor m_MaterialEditor;

        // Foldouts
        bool m_PoolingOptionsFoldout;
        bool m_ProjectionOptionsFoldout;

        // Properties
        SerializedProperty m_PoolingEnabledProp;
        SerializedProperty m_InstanceCountProp;
        SerializedProperty m_ProjectionDepthProp;
#endregion

#region Constructors
        public DecalDataEditor()
        {
            // Set data
            m_PoolingOptionsFoldout = true;
            m_ProjectionOptionsFoldout = true;
        }
#endregion

#region State
        void OnEnable()
        {
            // Set data
            m_Target = target as DecalData;
            if(!m_Target.isImported)
                return;

            // Get Properties
            m_PoolingEnabledProp = serializedObject.FindProperty(PropertyNames.PoolingEnabled);
            m_InstanceCountProp = serializedObject.FindProperty(PropertyNames.InstanceCount);
            m_ProjectionDepthProp = serializedObject.FindProperty(PropertyNames.ProjectionDepth);

            // Create Editors
            m_MaterialEditor = Editor.CreateEditor(m_Target.material) as MaterialEditor;
        }

        void OnDisable ()
        {
            // Cleanup
            if(m_MaterialEditor != null)
            {
                DestroyImmediate(m_MaterialEditor);
            }
        }
#endregion

#region GUI
        public override void OnInspectorGUI()
        {
            // Test for target imported
            if(!m_Target.isImported)
            {
                EditorGUILayout.HelpBox("Waiting for Asset import...", MessageType.Info);
                return;
            }
            
            // Setup
            serializedObject.Update();

            // Pooling Options
            m_PoolingOptionsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_PoolingOptionsFoldout, Styles.PoolingOptions);
            if(m_PoolingOptionsFoldout)
            {
                DrawPoolingOptions();
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            // Projection Options
            m_ProjectionOptionsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_ProjectionOptionsFoldout, Styles.ProjectionOptions);
            if(m_ProjectionOptionsFoldout)
            {
                DrawProjectionOptions();
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            // Material Editor
            DrawMaterialEditor();

            // Finalize
            serializedObject.ApplyModifiedProperties();
        }

        void DrawPoolingOptions()
        {
            // Enabled
            EditorGUILayout.PropertyField(m_PoolingEnabledProp, Styles.PoolingEnabled);
            
            // Instance Count
            using(var disabledScope = new EditorGUI.DisabledScope(!m_PoolingEnabledProp.boolValue))
            {
                EditorGUILayout.PropertyField(m_InstanceCountProp, Styles.InstanceCount);
            }
        }

        void DrawProjectionOptions()
        {
            // Projection Depth
            EditorGUILayout.PropertyField(m_ProjectionDepthProp, Styles.Depth);
        }

        void DrawMaterialEditor()
        {
            // Default Material Editor
            EditorGUILayout.Space();
            m_MaterialEditor.DrawHeader();
            m_MaterialEditor.OnInspectorGUI();
        }
#endregion
    }
}
