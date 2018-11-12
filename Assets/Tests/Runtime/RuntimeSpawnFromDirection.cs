using UnityEngine;
using kTools.Decals;

namespace kTools.Decals.Tests
{
	[AddComponentMenu("kTools/Tests/Decals/RuntimeSpawnFromDirection")]
	public class RuntimeSpawnFromDirection : MonoBehaviour
	{
		public ScriptableDecal decalData;

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
					DecalSystem.GetDecal(hit.point, -hit.normal, decalData, true);
			}
		}
	}
}
