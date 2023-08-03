using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

namespace kTools.Decals.Editor
{
    sealed class DecalSettingsProvider : SettingsProvider
    {
        private SerializedObject m_Settings;

        class Styles
        {
            public static readonly GUIContent EnablePerChannelDecals = new GUIContent("Enable Per Channel Decals",
                "If true, deferred Decals can write to individual GBuffer channels. Increases overhead as this requires duplicating all GBuffers");
        }
        
        public DecalSettingsProvider(string path, SettingsScope scope = SettingsScope.Project)
            : base(path, scope) {}

        public static bool IsSettingsAvailable()
        {
            return Resources.Load<DecalSettings>("DecalSettings") != null;
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            m_Settings = DecalSettings.GetSerializedSettings();
        }

        public override void OnGUI(string searchContext)
        {
            var previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = previousLabelWidth * 2;

            EditorGUILayout.PropertyField(m_Settings.FindProperty("m_EnablePerChannelDecals"), Styles.EnablePerChannelDecals);
            m_Settings?.ApplyModifiedProperties();

            EditorGUIUtility.labelWidth = previousLabelWidth;
        }

        // Register the SettingsProvider
        [SettingsProvider]
        public static SettingsProvider Create()
        {
            if (IsSettingsAvailable())
            {
                var provider = new DecalSettingsProvider("Project/kDecals", SettingsScope.Project);
                
                provider.keywords = GetSearchKeywordsFromGUIContentProperties<Styles>();
                return provider;
            }
            
            return null;
        }
    }
}
