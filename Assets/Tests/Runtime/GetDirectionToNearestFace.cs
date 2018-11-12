using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kTools.Decals.Tests
{
    [ExecuteInEditMode]
    [AddComponentMenu("kTools/Tests/Decals/GetDirectionToNearestFace")]
    public class GetDirectionToNearestFace : MonoBehaviour
    {
        public Decal decal;

        private Vector3 m_Direction;
        private Vector3 m_HitPoint;

        void Update () 
		{
            if(decal == null)
                return;

            m_Direction = DecalUtil.GetDirectionToNearestFace(decal, out m_HitPoint);
		}

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if(decal == null)
                return;
                
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(decal.transform.position, 0.05f);
            Gizmos.DrawSphere(m_HitPoint, 0.05f);
            Gizmos.DrawLine(decal.transform.position, m_HitPoint);
        }
#endif
    }
}
