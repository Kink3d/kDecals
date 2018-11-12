using UnityEngine;
using UnityEditor;
using kTools.Decals;

namespace kTools.DecalsEditor
{
    [CustomEditor(typeof(Decal))]
    public class DecalEditor : Editor
    {
        // -------------------------------------------------- //
        //                    EDITOR STYLES                   //
        // -------------------------------------------------- //

        internal class Styles
        {
            public static GUIContent propertiesText = EditorGUIUtility.TrTextContent("Properties");
            public static GUIContent definitionText = EditorGUIUtility.TrTextContent("Definition");
            public static GUIContent toolsText = EditorGUIUtility.TrTextContent("Tools");
        }

        // -------------------------------------------------- //
        //                   PRIVATE FIELDS                   //
        // -------------------------------------------------- //

        Decal m_ActualTarget;

        // -------------------------------------------------- //
        //                   PUBLIC METHODS                   //
        // -------------------------------------------------- //

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawProperties();
            DrawTools();
            serializedObject.ApplyModifiedProperties();
        }

        // -------------------------------------------------- //
        //                   PRIVATE METHODS                  //
        // -------------------------------------------------- //

        private void OnEnable()
        {
            m_ActualTarget = (Decal)target;
        }

        // Draw Properties fields section
        private void DrawProperties()
        {
            EditorGUILayout.LabelField(Styles.propertiesText, EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            var decalData = (ScriptableDecal)EditorGUILayout.ObjectField(Styles.definitionText, m_ActualTarget.decalData, typeof(ScriptableDecal), false);
            if (EditorGUI.EndChangeCheck())
            {
                m_ActualTarget.SetData(decalData);
            }
            EditorGUILayout.Space();
        }

        // Draw Tools fields section
        private void DrawTools()
        {
            EditorGUILayout.LabelField(Styles.toolsText, EditorStyles.boldLabel);
            if(GUILayout.Button("Move to nearest face"))
                OnClickMoveToNearestFace();
            if(GUILayout.Button("Orientate to nearest face"))
                OnClickOrientateToNearestFace();
            if(GUILayout.Button("Snap to nearest face"))
                OnClickSnapToNearestFace();
            EditorGUILayout.Space();
        }

        // Called when "Orientate to nearest face" button is clicked
        private void OnClickMoveToNearestFace()
        {
            Vector3 position;
            DecalUtil.GetDirectionToNearestFace(m_ActualTarget, out position);
            m_ActualTarget.SetTransform(position, m_ActualTarget.transform.rotation, m_ActualTarget.transform.lossyScale);
            m_ActualTarget.SetData(m_ActualTarget.decalData);
        }

        // Called when "Orientate to nearest face" button is clicked
        private void OnClickOrientateToNearestFace()
        {
            Vector3 directionVector = DecalUtil.GetDirectionToNearestFace(m_ActualTarget);
            m_ActualTarget.SetTransform(m_ActualTarget.transform.position, directionVector, m_ActualTarget.transform.lossyScale);
            m_ActualTarget.SetData(m_ActualTarget.decalData);
        }

        // Called when "Snap to nearest face" button is clicked
        private void OnClickSnapToNearestFace()
        {
            Vector3 position;
            Vector3 directionVector = DecalUtil.GetDirectionToNearestFace(m_ActualTarget, out position);
            m_ActualTarget.SetTransform(position, directionVector, m_ActualTarget.transform.lossyScale);
            m_ActualTarget.SetData(m_ActualTarget.decalData);
        }
    }
}
