using UnityEngine;
using System.Collections;

namespace SimpleTools.Decals
{
	[CreateAssetMenu(fileName = "DecalData", menuName = "SimpleTools/DecalData", order = 1)]
	public class DecalData : ScriptableObject 
	{
		public BlendMode blendMode;
		public Texture2D texture;
		public int maxInstances = 100;
	}
}
