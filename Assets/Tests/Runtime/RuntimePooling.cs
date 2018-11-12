using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kTools.Decals.Tests
{
    [AddComponentMenu("kTools/Tests/Decals/RuntimePooling")]
    public class RuntimePooling : MonoBehaviour
    {
        public ScriptableDecal decalData;
        public float spawnInterval = 1.0f;
        public Transform[] locations;

        private float m_Timer;
        private int m_CurrentLocationIndex = 0;

        void Update () 
		{
            if(m_Timer < spawnInterval)
            {
                m_Timer += Time.deltaTime;
                return;
            }

            m_Timer = 0.0f;
			SpawnDecal();
		}

        void SpawnDecal()
        {
            DecalSystem.GetDecal(locations[m_CurrentLocationIndex].position,
                locations[m_CurrentLocationIndex].rotation, decalData, true);

            if(m_CurrentLocationIndex < locations.Length - 1)
                m_CurrentLocationIndex++;
            else
                m_CurrentLocationIndex = 0;
        }
    }
}
