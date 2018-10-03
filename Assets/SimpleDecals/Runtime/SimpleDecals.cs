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
        // Public Fields

		public int maxDecals = 100;

		// -------------------------------------------------------------------
        // Data

		private Decal[] decals;

		// -------------------------------------------------------------------
        // Unity Methods

		private void OnEnable()
		{
			InitializeDecals();
		}

		// -------------------------------------------------------------------
        // Initialization

		private void InitializeDecals()
		{
			decals = new Decal[maxDecals];
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
		}

		// -------------------------------------------------------------------
        // Instances

		private bool TryGetInstance(out Decal decal)
		{
			for (int i = 0; i < decals.Length; i++)
            {
                if (!decals[i].gameObject.activeSelf)
                {
                    decal = decals[i];
                    return true;
                }
            }
			decal = null;
			return false;
		}

		public Decal CreateDecal(Transform hitObj, Vector3 position, Vector3 rotation, Material material)
        {
            Decal decal = null;
            bool createdInstance = TryGetInstance(out decal);
            if (createdInstance)
			{
                decal.Initialize(hitObj, position, rotation, material);
			}
			return decal;
        }
	}
}
