using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace kTools.Decals
{
    public sealed class DecalSettings : ScriptableObject
    {
        private const string kDirectory = "kDecals/Resources";
        private const string kName = "DecalSettings";

        [SerializeField]
        private bool m_EnablePerChannelDecals;

        public DecalSettings()
        {
            m_EnablePerChannelDecals = false;
        }

        public bool enablePerChannelDecals
        {
            get => m_EnablePerChannelDecals;
            set => m_EnablePerChannelDecals = value;
        }

        public static DecalSettings GetOrCreateSettings()
        {
            var settings = Resources.Load<DecalSettings>(kName);
            
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<DecalSettings>();

                #if UNITY_EDITOR
                var fullDirectory = $"{Application.dataPath}/{kDirectory}";
                if(!Directory.Exists(fullDirectory))
                    Directory.CreateDirectory(fullDirectory);

                var path = $"Assets/{kDirectory}/{kName}.asset";
                UnityEditor.AssetDatabase.CreateAsset(settings, path);
                UnityEditor.AssetDatabase.SaveAssets();
                #endif
            }

            return settings;
        }

        #if UNITY_EDITOR
        public static UnityEditor.SerializedObject GetSerializedSettings()
        {
            return new UnityEditor.SerializedObject(GetOrCreateSettings());
        }
        #endif
    }
}
