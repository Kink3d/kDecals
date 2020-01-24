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
        float m_Depth;

        [SerializeField]
        float m_DepthFalloff;

        [SerializeField]
        float m_Angle;

        [SerializeField]
        float m_AngleFalloff;

        [SerializeField]
        int m_LayerMask;

        [SerializeField]
        int m_SortingOrder;
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
            m_Depth = 1.0f;
            m_DepthFalloff = 0.5f;
            m_Angle = 90.0f;
            m_AngleFalloff = 0.5f;
            m_LayerMask = -1;
            m_SortingOrder = 0;
        }
#endregion

#region Properties
        /// <summary>True if Decal asset has been processed.</summary>
        public bool isImported => m_IsImported;

        /// <summary>Material used for Decal rendering.</summary>
        public Material material => m_Material;

        /// <summary>If enabled all Decals created via DecalSystem will be pre-generated and taken from a Pool.</summary>
        public bool poolingEnabled => m_PoolingEnabled;

        /// <summary>How many instances of this Decal will be created in the Pool.</summary>
        public int instanceCount => m_InstanceCount;

        /// <summary>How far the decal projection draw on Z axis.</summary>
        public float depth => m_Depth;

        /// <summary>How much the decal should blend transparency towards its Depth value.</summary>
        public float depthFalloff => m_DepthFalloff;

        /// <summary>Maximum angle between surface and Decal forward vector.</summary>
        public float angle => m_Angle;

        /// <summary>How much the Decal should blend transparency towards its Angle value.</summary>
        public float angleFalloff => m_AngleFalloff;

        /// <summary>Which layers the Decal is applied to.</summary>
        public LayerMask layerMask => m_LayerMask;

        /// <summary>Decals with higher values are drawn on top of ones with lower values.</summary>
        public int sortingOrder => m_SortingOrder;
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
