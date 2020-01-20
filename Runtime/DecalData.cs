using UnityEngine;

namespace kTools.Decals
{
#region AssetPostprocessor
#if UNITY_EDITOR
    using UnityEditor;

    sealed class DecalDataPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string str in importedAssets)
            {
                // Test AssetDatabase to determine if DecalData
                var decalData = AssetDatabase.LoadAssetAtPath(str, typeof(DecalData)) as DecalData; 
                if(decalData != null)
                {
                    // Force Postprocess asset
                    decalData.OnAssetImport();
                    return;
                }
            }
        }
    }
#endif
#endregion
    
    /// <summary>
    /// DecalData ScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "New DecalData", menuName = "kTools/Decals/DecalData", order = 1)]
    public sealed class DecalData : ScriptableObject
    {
#region Serialized Fields
        [SerializeField]
        bool m_IsImported;

        [SerializeField]
        Material m_Material;

        [SerializeField]
        bool m_PoolingEnabled;

        [SerializeField]
        int m_InstanceCount;

        [SerializeField]
        float m_ProjectionDepth;
#endregion

#region Fields
        const string kDefaultShader = "kDecals/Lit";
#endregion

#region Constructors
        public DecalData()
        {
            // Set data
            m_PoolingEnabled = false;
            m_InstanceCount = 32;
        }
#endregion

#region Properties
        public bool isImported => m_IsImported;
        public Material material => m_Material;
        public bool poolingEnabled => m_PoolingEnabled;
        public int instanceCount => m_InstanceCount;
        public float projectionDepth => m_ProjectionDepth;
#endregion

#region Asset Processing
#if UNITY_EDITOR
        public void OnAssetImport()
        {
            // Create Material
            if(m_Material == null)
            {
                m_Material = new Material(Shader.Find(kDefaultShader));
                AssetDatabase.AddObjectToAsset(m_Material, this);
            }

            // Update Material
            m_Material.name = this.name;
            AssetDatabase.SaveAssets();

            // Finalize
            m_IsImported = true;
        }
#endif
#endregion
    }
}
