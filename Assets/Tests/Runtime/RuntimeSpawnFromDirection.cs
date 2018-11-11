using UnityEngine;
using kTools.Decals;

namespace kTools.Decals.Tests
{
	public class RuntimeSpawnFromDirection : MonoBehaviour
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
					DecalSystem.GetDecal(hit.point, -hit.normal, Vector2.one, decalData, true);
			}
		}
	}
}
