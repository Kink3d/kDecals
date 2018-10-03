using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

		public DecalPool InitializePool(DecalData decalData)
		{
			Decal[] decals = new Decal[decalData.maxInstances];
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
				Decal decal = transform.gameObject.AddComponent<Decal>();
				decals[i] = decal;
				obj.SetActive(false);
            }

			DecalPool pool = new DecalPool(decalData, decals, initTime);
			pools.Add(pool);
			return pool;
		}

		private DecalPool GetPool(DecalData decalData)
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

		public Decal CreateDecal(Transform hitObj, Vector3 position, Vector3 rotation, DecalData decalData)
        {
			return CreateDecal(hitObj, position, rotation, Vector2.one, decalData);
        }

		public Decal CreateDecal(Transform hitObj, Vector3 position, Vector3 rotation, Vector2 scale, DecalData decalData)
        {
            Decal decal = null;
            bool createdInstance = TryGetInstance(decalData, out decal);
            if (createdInstance)
			{
                decal.Initialize(hitObj, position, rotation, scale, decalData);
			}
			return decal;
        }

		private bool TryGetInstance(DecalData decalData, out Decal decal)
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
		public DecalPool (DecalData decalData, Decal[] decals, float[] initTime)
		{
			this.decalData = decalData;
			this.decals = decals;	
			this.initTimes = initTime;
		}

		public DecalData decalData;
		public Decal[] decals;
		public float[] initTimes;
	}
}
