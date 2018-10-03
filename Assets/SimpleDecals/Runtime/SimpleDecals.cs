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
        // Initialization

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

		private DecalPool InitializePool(DecalData decalData)
		{
			Decal[] decals = new Decal[decalData.maxInstances];
			for (int i = 0; i < decals.Length; i++)
            {
				GameObject obj = new GameObject();
                Transform transform = obj.transform;
				transform.parent = this.transform;
				transform.position = Vector3.zero;
				transform.rotation = Quaternion.identity;
				transform.localScale = transform.InverseTransformVector(Vector3.zero);
				Decal decal = transform.gameObject.AddComponent<Decal>();
				decals[i] = decal;
				obj.SetActive(false);
            }

			DecalPool pool = new DecalPool(decalData, decals);
			pools.Add(pool);
			return pool;
		}

		// -------------------------------------------------------------------
        // Decal Instances

		public Decal CreateDecal(Transform hitObj, Vector3 position, Vector3 rotation, DecalData decalData)
        {
            Decal decal = null;
            bool createdInstance = TryGetInstance(decalData, out decal);
            if (createdInstance)
			{
                decal.Initialize(hitObj, position, rotation, decalData);
			}
			return decal;
        }

		private bool TryGetInstance(DecalData decalData, out Decal decal)
		{
			DecalPool pool = GetPool(decalData);
			for (int i = 0; i < pool.decals.Length; i++)
            {
                if (!pool.decals[i].gameObject.activeSelf)
                {
                    decal = pool.decals[i];
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
		public DecalPool (DecalData decalData, Decal[] decals)
		{
			this.decalData = decalData;
			this.decals = decals;	
		}

		public DecalData decalData;
		public Decal[] decals;
	}
}
