using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kTools.Decals.Tests
{
    [AddComponentMenu("kTools/Tests/Decals/RuntimeRefresh")]
    public class RuntimeRefresh : MonoBehaviour
    {
        public ScriptableDecal[] decalDatas;
        public Transform location;
        public float refreshInterval = 1.0f;

        private Decal m_Decal;
        private float m_Timer;
        private int m_CurrentDataIndex = 0;

        void Start()
        {
            m_Decal = DecalSystem.GetDecal(location.position, location.rotation, decalDatas[m_CurrentDataIndex], false);
            m_CurrentDataIndex++;
        }

        void Update () 
		{
            if(m_Timer < refreshInterval)
            {
                m_Timer += Time.deltaTime;
                return;
            }

            m_Timer = 0.0f;
			RefreshDecal();
		}

        void RefreshDecal()
        {
            m_Decal.SetData(decalDatas[m_CurrentDataIndex]);

            if(m_CurrentDataIndex < decalDatas.Length - 1)
                m_CurrentDataIndex++;
            else
                m_CurrentDataIndex = 0;
        }
    }
}
