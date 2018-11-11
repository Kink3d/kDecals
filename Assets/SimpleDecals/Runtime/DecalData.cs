using System;
using UnityEngine;
using System.Linq;

namespace kTools.Decals
{
    [CreateAssetMenu(fileName = "New DecalData", menuName = "kTools/kDecalData", order = 1)]
	public class DecalData : ScriptableObject 
    {
        // -------------------------------------------------- //
        //                   PRIVATE FIELDS                   //
        // -------------------------------------------------- //

        [SerializeField] private Type m_DecalDefinitionType;

        [SerializeField] private int m_MaxInstances = 100;
        public int maxInstances
        {
            get { return m_MaxInstances; }
        }

        [SerializeField] private DecalDefinition m_DecalDefinition;
        public DecalDefinition decalDefinition
        {
            get { return m_DecalDefinition; }
        }

        // -------------------------------------------------- //
        //                    CONSTRUCTORS                    //
        // -------------------------------------------------- //

        public DecalData()
        {
            // Init to DecalDefintiion index Unlit/Blend
            ChangeDefinition(typeof(BlendDecalDefinition));
        }

        // -------------------------------------------------- //
        //                   PUBLIC METHODS                   //
        // -------------------------------------------------- //

        /// <summary>
        /// Change the active DecalDefinition.
        /// </summary>
        /// <param name="value">New DecalDefinition type.</param>
        public void ChangeDefinition(Type value)
        {
            if(value == m_DecalDefinitionType)
                return;
            
            m_DecalDefinitionType = value;
            m_DecalDefinition = (DecalDefinition)Activator.CreateInstance(value);
        }
    }
}
