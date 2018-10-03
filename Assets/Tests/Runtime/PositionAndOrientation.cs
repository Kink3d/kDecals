using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleTools.Decals.Tests
{
	[RequireComponent(typeof(SimpleDecals))]
	public class PositionAndOrientation : MonoBehaviour
	{
		public DecalData decalData;

		void Update () 
		{
			CheckPlacement();
		}

		private void CheckPlacement()
		{
			if(Input.GetMouseButtonUp(0))
			{
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				
				if (Physics.Raycast(ray, out hit)) 
				{
					Decal decal = SimpleDecals.Instance.CreateDecal(hit.transform, hit.point, hit.normal, decalData);
					if(decal == null)
						Debug.LogError("Failed to create Decal");
				}
			}
		}
	}
}
