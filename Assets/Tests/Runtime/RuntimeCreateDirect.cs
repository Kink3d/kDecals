using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kTools.Decals.Tests
{
    [AddComponentMenu("kTools/Tests/Decals/RuntimeCreateDirect")]
    public class RuntimeCreateDirect : MonoBehaviour
    {
        public ScriptableDecal decalData;
        public Transform location;

        void Start () 
		{
            Decal decal = DecalSystem.CreateDecalDirect(decalData);
            decal.SetTransform(location.position, location.rotation, Vector2.one);
            decal.SetData(decalData);
            decal.transform.localScale = Vector3.one;
		}
    }
}
