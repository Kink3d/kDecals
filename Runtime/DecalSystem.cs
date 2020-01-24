using System.Collections.Generic;
using UnityEngine;
using kTools.Pooling;

namespace kTools.Decals
{
    /// <summary>
    /// Static system for creating and managing Decals.
    /// </summary>
    public static class DecalSystem
    {
#region Fields
        static readonly List<Decal> m_Decals;
#endregion

#region Constructors
        static DecalSystem()
        {
            m_Decals = new List<Decal>();
        }
#endregion

#region Properties
        internal static List<Decal> decals => m_Decals;
#endregion

#region Pool
        /// <summary>
        /// Create Pool for Decals using DecalData as key.
        /// </summary>
        /// <param name="decalData">DecalData to create Decals from.</param>
        public static void CreateDecalPool(DecalData decalData)
        {
            // Test for pooling enabled
            if(!decalData.poolingEnabled)
            {
                Debug.LogWarning($"Pooling is not enabled for DecalData ({decalData.name})");
                return;
            }

            // Create Pool
            var decal = CreateDecal(decalData);
            PoolingSystem.CreatePool(decalData, decal, decalData.instanceCount);

            // Cleanup
            DestroyGameObject(decal.gameObject);
        }

        /// <summary>
        /// Destroy Pool of Decals.
        /// </summary>
        /// <param name="decalData">Key for Pool to destroy.</param>
        public static void DestroyDecalPool(DecalData decalData)
        {
            // Test for pooling enabled
            if(!decalData.poolingEnabled)
            {
                Debug.LogWarning($"Pooling is not enabled for DecalData ({decalData.name})");
                return;
            }

            // Destroy Pool
            PoolingSystem.DestroyPool<Decal>(decalData);
        }

        /// <summary>
        /// Tests whether Pool exists for DecalData.
        /// </summary>
        /// <param name="decalData">Key to test for.</param>
        /// <returns>True if Pool exists.</returns>
        public static bool HasDecalPool(DecalData decalData)
        {
            // Test for pooling enabled
            if(!decalData.poolingEnabled)
            {
                Debug.LogWarning($"Pooling is not enabled for DecalData ({decalData.name})");
                return false;
            }

            // Test for matching Pool
            return PoolingSystem.HasPool<Decal>(decalData);
        }
#endregion

#region Decal
        /// <summary>
        /// Get new Decal and set Transform. If Pooling is enabled on DecalData, Decal will be taken from Pool.
        /// </summary>
        /// <param name="decalData">DecalData to create Decal from.</param>
        /// <param name="position">World space position for Decal.</param>
        /// <param name="direction">World space forward direction for Decal.</param>
        /// <param name="scale">Local space scale for Decal.</param>
        /// <returns></returns>
        public static Decal GetDecal(DecalData decalData, Vector3 position, Vector3 direction, Vector3 scale)
        {
            var decal = GetDecal(decalData);
            decal.SetTransform(position, direction, scale);
            return decal;
        }

        /// <summary>
        /// Get new Decal. If Pooling is enabled on DecalData, Decal will be taken from Pool.
        /// </summary>
        /// <param name="decalData">DecalData to create Decal from.</param>
        /// <returns></returns>
        public static Decal GetDecal(DecalData decalData)
        {
            // Test for pooling enabled
            if(decalData.poolingEnabled)
            {
                // Create Pool
                if(!HasDecalPool(decalData))
                {
                    CreateDecalPool(decalData);
                }

                // Get Decal from Pool
                Decal decal;
                PoolingSystem.TryGetInstance(decalData, out decal);
                return decal;
            }
            
            // Create new Decal
            return CreateDecal(decalData);
        }
        
        /// <summary>
        /// Remove existing Decal. If Pooling is enabled on DecalData, Decal will be returned to Pool.
        /// </summary>
        /// <param name="decal">Decal to remove.</param>
        public static void RemoveDecal(Decal decal)
        {
            // Test for pooling enabled
            var key = decal.decalData;
            if(key.poolingEnabled)
            {
                // Try to return Decal to Pool
                if(HasDecalPool(key))
                {
                    PoolingSystem.ReturnInstance<Decal>(key, decal);
                    return;
                }
            }

            // Destroy decal
            DestroyGameObject(decal.gameObject);
        }

        static Decal CreateDecal(DecalData decalData)
        {
            // Create new Decal
            var name = decalData.name;
            var obj = new GameObject(name, typeof(Decal));
            var decal = obj.GetComponent<Decal>();
            decal.decalData = decalData;
            return decal;
        }
#endregion

#region Registration
        internal static void RegisterDecal(Decal decal)
        {
            if(m_Decals.Contains(decal))
                return;

            // Track Decal
            m_Decals.Add(decal);
        }

        internal static void UnregisterDecal(Decal decal)
        {
            if(!m_Decals.Contains(decal))
                return;

            // Untrack Decal
            m_Decals.Remove(decal);
        }
#endregion

#region GameObject
        static void DestroyGameObject(GameObject gameObject)
        {
            #if UNITY_EDITOR
            Object.DestroyImmediate(gameObject);
            #else
            Object.Destroy(gameObject);
            #endif
        }
#endregion
    }
}
