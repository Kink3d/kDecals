using UnityEngine;
using UnityEditor;
using kTools.Decals;

namespace kTools.DecalsEditor
{
    [CustomEditor(typeof(DecalData))]
    public class DecalDataEditor : Editor
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

        DecalData m_ActualTarget;
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
            m_ActualTarget = (DecalData)target;
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
                EditorGUILayout.SelectableLabel(m_ActualTarget.decalDefinition.GetType().GetAttribute<DecalDefinitionAttribute>().menuItem, EditorStyles.textField, 
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
            foreach(DecalProperty prop in m_ActualTarget.decalDefinition.properties)
            {
                if(prop as TextureDecalProperty != null)
                {
                    var textureProp = prop as TextureDecalProperty;
                    textureProp.value = (Texture2D)EditorGUILayout.ObjectField(textureProp.displayName, textureProp.value, typeof(Texture2D), false);
                }
                else if(prop as ColorDecalProperty != null)
                {
                    var colorProp = prop as ColorDecalProperty;
                    colorProp.value = EditorGUILayout.ColorField(colorProp.displayName, colorProp.value);
                }
                else if(prop as FloatDecalProperty != null)
                {
                    var floatProp = prop as FloatDecalProperty;
                    floatProp.value = EditorGUILayout.FloatField(floatProp.displayName, floatProp.value);
                }
                else if(prop as VectorDecalProperty != null)
                {
                    var vectorProp = prop as VectorDecalProperty;
                    vectorProp.value = EditorGUILayout.Vector4Field(vectorProp.displayName, vectorProp.value);
                }
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
