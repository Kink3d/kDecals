using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using kTools.Decals;

namespace SimpleTools.Decals
{
	public class SimpleDecals : MonoBehaviour
	{
		// -------------------------------------------------------------------
        // Singleton

        private static SimpleDecals _Instance;
        public static SimpleDecals Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = FindObjectOfType<SimpleDecals>();
                return _Instance;
            }
        }

		// -------------------------------------------------------------------
        // Data

		private List<DecalPool> pools = new List<DecalPool>();

		// -------------------------------------------------------------------
        // Pool Managementas

		public DecalPool InitializePool(kDecalData decalData)
		{
			kDecal[] decals = new kDecal[decalData.maxInstances];
			float[] initTime = new float[decalData.maxInstances];
			for (int i = 0; i < decals.Length; i++)
            {
				GameObject obj = new GameObject();
				obj.name = string.Format("Decal_{0}", decalData.name);
                Transform transform = obj.transform;
				transform.parent = this.transform;
				transform.position = Vector3.zero;
				transform.rotation = Quaternion.identity;
				transform.localScale = transform.InverseTransformVector(Vector3.zero);
				kDecal decal = transform.gameObject.AddComponent<kDecal>();
				decals[i] = decal;
				obj.SetActive(false);
            }

			DecalPool pool = new DecalPool(decalData, decals, initTime);
			pools.Add(pool);
			return pool;
		}

		private DecalPool GetPool(kDecalData decalData)
		{
			for (int i = 0; i < pools.Count; i++)
            {
                if (pools[i].decalData == decalData)
                {
                    return pools[i];
                }
            }
			return InitializePool(decalData);
		}

		private void ValidatePool(DecalPool pool)
		{
			int oldestIndex = 0;
			float oldestTime = Mathf.Infinity;
			for (int i = 0; i < pool.decals.Length; i++)
            {
                if (!pool.decals[i].gameObject.activeSelf)
					return;

				if(pool.initTimes[i] < oldestTime)
				{
					oldestTime = pool.initTimes[i];
					oldestIndex = i;
				}
            }

			pool.decals[oldestIndex].gameObject.SetActive(false);
		}

		// -------------------------------------------------------------------
        // Decal Instance Management

		public kDecal CreateDecal(Transform hitObj, Vector3 position, Vector3 rotation, kDecalData decalData)
        {
			return CreateDecal(hitObj, position, rotation, Vector2.one, decalData);
        }

		public kDecal CreateDecal(Transform hitObj, Vector3 position, Vector3 rotation, Vector2 scale, kDecalData decalData)
        {
            kDecal decal = null;
            bool createdInstance = TryGetInstance(decalData, out decal);
            if (createdInstance)
			{
				decal.SetDecalActive(true);
				decal.SetDecalTransform(position, rotation, scale);
				decal.SetDecalData(decalData);
			}
			return decal;
        }

		private bool TryGetInstance(kDecalData decalData, out kDecal decal)
		{
			DecalPool pool = GetPool(decalData);
			ValidatePool(pool);
			for (int i = 0; i < pool.decals.Length; i++)
            {
                if (!pool.decals[i].gameObject.activeSelf)
                {
                    decal = pool.decals[i];
					pool.initTimes[i] = Time.realtimeSinceStartup;
                    return true;
                }
            }
			decal = null;
			return false;
		}
	}

	// -------------------------------------------------------------------
    // Data Structures

	[Serializable]
	public class DecalPool
	{
		public DecalPool (kDecalData decalData, kDecal[] decals, float[] initTime)
		{
			this.decalData = decalData;
			this.decals = decals;	
			this.initTimes = initTime;
		}

		public kDecalData decalData;
		public kDecal[] decals;
		public float[] initTimes;
	}
}
