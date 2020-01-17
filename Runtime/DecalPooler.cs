using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kTools.Decals
{
	[AddComponentMenu("")]
	public sealed class DecalPooler : MonoBehaviour
	{
		// -------------------------------------------------- //
        //                     SINGELTON                      //
        // -------------------------------------------------- //

        private static DecalPooler _Instance;
        public static DecalPooler Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = FindObjectOfType<DecalPooler>();
                return _Instance;
            }
        }

		// -------------------------------------------------- //
        //                   PRIVATE FIELDS                   //
        // -------------------------------------------------- //

		private List<DecalPool> pools = new List<DecalPool>();
		
		// -------------------------------------------------- //
        //                   PUBLIC METHODS                   //
        // -------------------------------------------------- //

		/// <summary>
        /// Try to get a Decal instance from pools.
        /// </summary>
        /// <param name="decalData">DecalData to get an instance of.</param>
        /// <param name="decal">Decal instance out.</param>
		public bool TryGetInstance(ScriptableDecal decalData, out Decal decal)
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

		// -------------------------------------------------- //
        //                  INTERNAL METHODS                  //
        // -------------------------------------------------- //

		// Initialize a new DecalPool
		private DecalPool InitializePool(ScriptableDecal decalData)
		{
			Decal[] decals = new Decal[decalData.maxInstances];
			float[] initTime = new float[decalData.maxInstances];
			for (int i = 0; i < decals.Length; i++)
            {
				Decal decal = DecalSystem.CreateDecalDirect(decalData);
            	decal.transform.localScale = this.transform.InverseTransformVector(Vector3.one);
				decal.transform.SetParent(this.transform);
				decals[i] = decal;
				decal.gameObject.SetActive(false);
            }

			DecalPool pool = new DecalPool(decalData, decals, initTime);
			pools.Add(pool);
			return pool;
		}
		
		// Get a DecalPool by DecalData
		private DecalPool GetPool(ScriptableDecal decalData)
		{
			for (int i = 0; i < pools.Count; i++)
            {
                if (pools[i].decalData == decalData)
                    return pools[i];
            }
			return InitializePool(decalData);
		}

		// If no available instances in the pool disable the oldest
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
	}

	// -------------------------------------------------- //
	//              SERIALIZABLE DATA CLASSES             //
	// -------------------------------------------------- //

	[Serializable]
	public class DecalPool
	{
		public DecalPool (ScriptableDecal decalData, Decal[] decals, float[] initTime)
		{
			this.decalData = decalData;
			this.decals = decals;	
			this.initTimes = initTime;
		}

		public ScriptableDecal decalData;
		public Decal[] decals;
		public float[] initTimes;
	}
}
