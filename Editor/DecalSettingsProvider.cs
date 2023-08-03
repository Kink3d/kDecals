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

            public static readonly GUIContent GBufferUpdateFrequency = new GUIContent("GBuffer Update Frequency",
                "Sets how often the copied GBuffers should be updated. This affects the underlying values when blending per-channel Decals.\nNever: Decals are never in the underlying values\nPer Decal: Individual Decals can be written on a case by case basis\nAlways: GBuffers are updated after every Decal (not recommended)");
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
            EditorGUILayout.PropertyField(m_Settings.FindProperty("m_GBufferUpdateFrequency"), Styles.GBufferUpdateFrequency);
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
