using UnityEngine;
using UnityEditor;
using kTools.Decals;

namespace kTools.DecalsEditor
{
    [CustomEditor(typeof(ScriptableDecal))]
    public class ScriptableDecalEditor : Editor
    {
        // -------------------------------------------------- //
        //                    EDITOR STYLES                   //
        // -------------------------------------------------- //

        internal class Styles
        {
            public static GUIContent commonText = EditorGUIUtility.TrTextContent("Common");
            public static GUIContent definitionText = EditorGUIUtility.TrTextContent("Definition");
            public static GUIContent poolingText = EditorGUIUtility.TrTextContent("Pooling");
			public static GUIContent maxInstancesText = EditorGUIUtility.TrTextContent("Max Instances");
            public static GUIContent propertiesText = EditorGUIUtility.TrTextContent("Properties");
        }

        // -------------------------------------------------- //
        //                   PRIVATE FIELDS                   //
        // -------------------------------------------------- //

        ScriptableDecal m_ActualTarget;
        SerializedProperty m_MaxInstancesProp;

        // -------------------------------------------------- //
        //                   PUBLIC METHODS                   //
        // -------------------------------------------------- //

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawCommon();
            DrawPooling();
            DrawProperties();
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(m_ActualTarget);
        }

        // -------------------------------------------------- //
        //                   PRIVATE METHODS                  //
        // -------------------------------------------------- //

        private void OnEnable()
        {
            m_ActualTarget = (ScriptableDecal)target;
            m_MaxInstancesProp = serializedObject.FindProperty("m_MaxInstances");
        }

        // Draw Common fields section
        private void DrawCommon()
        {
            EditorGUILayout.LabelField(Styles.commonText, EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(Styles.definitionText, 
                    GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                
                if(GUILayout.Button("Select", 
                    GUILayout.Width(GUILayoutUtility.GetLastRect().width + 80), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                {
                    OnSelectDefinition();
                }
                var definitionType = System.Type.GetType(m_ActualTarget.decalDefinitionType);
                EditorGUILayout.SelectableLabel(definitionType.GetAttribute<DecalDefinitionAttribute>().menuItem, EditorStyles.textField, 
                    GUILayout.Width(GUILayoutUtility.GetLastRect().width + EditorGUIUtility.labelWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight));
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        // Draw Pooling fields section
        private void DrawPooling()
        {
            EditorGUILayout.LabelField(Styles.poolingText, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_MaxInstancesProp, Styles.maxInstancesText);
            EditorGUILayout.Space();
        }

        // Draw Properties fields section
        private void DrawProperties()
        {
            EditorGUILayout.LabelField(Styles.propertiesText, EditorStyles.boldLabel);
            foreach(SerializableDecalProperty prop in m_ActualTarget.serializedProperties)
            {
                if(prop.type == PropertyType.Texture)
                    prop.textureValue = (Texture2D)EditorGUILayout.ObjectField(prop.displayName, prop.textureValue, typeof(Texture2D), false);
                else if(prop.type == PropertyType.Color)
                    prop.colorValue = EditorGUILayout.ColorField(prop.displayName, prop.colorValue);
                else if(prop.type == PropertyType.Float)
                    prop.floatValue = EditorGUILayout.FloatField(prop.displayName, prop.floatValue);
                else if(prop.type == PropertyType.Vector)
                    prop.vectorValue = EditorGUILayout.Vector4Field(prop.displayName, prop.vectorValue);
                else if(prop.type == PropertyType.Keyword)
                    prop.boolValue = EditorGUILayout.Toggle(prop.displayName, prop.boolValue);
                else
                    Debug.LogError("Property is not a valid DecalProperty.");
            }
        }

        // Called when Select button is clicked
        private void OnSelectDefinition()
        {
            var menu = new GenericMenu();
            var definitionTypes = DecalUtil.GetAllAssemblySubclassTypes(typeof(DecalDefinition));

            foreach (var type in definitionTypes)
                menu.AddItem(new GUIContent(type.GetAttribute<DecalDefinitionAttribute>().menuItem), 
                    false, () => m_ActualTarget.ChangeDefinition(type));

            menu.ShowAsContext();
        }
    }
}
