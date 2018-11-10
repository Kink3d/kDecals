using UnityEngine;

namespace kTools.Decals
{
	[ExecuteInEditMode]
	public class DecalScale : MonoBehaviour
	{
		public kDecalData decalData;

		private void Update()
		{
			if(decalData)
			{
				DecalSystem.GetDecal(Vector3.zero, Vector3.up, new Vector2(1f, 1f), decalData, true);
				DecalSystem.GetDecal(new Vector3(0.25f, 0, 0), Vector3.back, new Vector2(0.5f, 1f), decalData, true);
				DecalSystem.GetDecal(new Vector3(0, -0.25f, 0), Vector3.left, new Vector2(1f, 0.5f), decalData, true);
			}
		}
	}
}
