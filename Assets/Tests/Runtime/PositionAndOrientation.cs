using UnityEngine;

namespace kTools.Decals
{
	public class PositionAndOrientation : MonoBehaviour
	{
		public kDecalData decalData;
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
					kDecal decal = DecalSystem.GetDecal(hit.point, hit.normal, new Vector2(width, height), decalData, true);
					if(decal == null)
						Debug.LogError("Failed to create Decal");
				}
			}
		}
	}
}
