using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kTools.Decals.Tests
{
    [AddComponentMenu("kTools/Tests/Decals/RuntimeSpawnFromRotation")]
    public class RuntimeSpawnFromRotation : MonoBehaviour
    {
        public ScriptableDecal decalData;
        public Transform[] locations;

        void Start () 
		{
            foreach(Transform t in locations)
			    SpawnDecal(t);
		}

        void SpawnDecal(Transform t)
        {
            DecalSystem.GetDecal(t.position, t.rotation, decalData, false);
        }
    }
}
