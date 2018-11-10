using System;
using UnityEngine;
using System.Linq;

namespace kTools.Decals
{
    [CreateAssetMenu(fileName = "New DecalData", menuName = "kTools/kDecalData", order = 1)]
	public class kDecalData : ScriptableObject 
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

        [SerializeField] private kDecalDefinition m_DecalDefinition;
        public kDecalDefinition decalDefinition
        {
            get { return m_DecalDefinition; }
        }

        // -------------------------------------------------- //
        //                    CONSTRUCTORS                    //
        // -------------------------------------------------- //

        public kDecalData()
        {
            // Init to DecalDefintiion index 0
            ChangeDefinition(0);
        }

        // -------------------------------------------------- //
        //                   PUBLIC METHODS                   //
        // -------------------------------------------------- //

        /// <summary>
        /// Change the active DecalDefinition.
        /// </summary>
        /// <param name="value">New DecalDefinition type.</param>
        public void ChangeDefinition(int value)
        {
            var editorTypes = kDecalUtil.GetAllAssemblySubclassTypes(typeof(kDecalDefinition));
            var selectedType = editorTypes.ElementAt(value);
            if(selectedType == m_DecalDefinitionType)
                return;
            
            m_DecalDefinitionType = selectedType;
            m_DecalDefinition = (kDecalDefinition)Activator.CreateInstance(selectedType);
        }
    }
}
