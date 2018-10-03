using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleTools.Decals.Tests
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(SimpleDecals))]
	public class DecalScale : MonoBehaviour
	{
		public DecalData decalData;

		private void Update()
		{
			if(decalData)
			{
				SimpleDecals.Instance.CreateDecal(this.transform, Vector3.zero, Vector3.up, new Vector2(1f, 1f), decalData);
				SimpleDecals.Instance.CreateDecal(this.transform, new Vector3(0.25f, 0, 0), Vector3.back, new Vector2(0.5f, 1f), decalData);
				SimpleDecals.Instance.CreateDecal(this.transform, new Vector3(0, -0.25f, 0), Vector3.left, new Vector2(1f, 0.5f), decalData);
			}
		}
	}
}
