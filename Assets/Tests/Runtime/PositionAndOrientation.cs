using UnityEngine;

namespace kTools.Decals
{
	public class PositionAndOrientation : MonoBehaviour
	{
		public DecalData decalData;
		public float width = 1;
		public float height = 1;

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
					DecalSystem.GetDecal(hit.point, hit.normal, new Vector2(width, height), decalData, true);
				}
			}
		}
	}
}
