using System;
using System.Collections.Generic;
using UnityEngine;

namespace kTools.Decals
{
    [Serializable]
    public struct DecalDefinitionContext
    {
        // -------------------------------------------------- //
        //                   PRIVATE FIELDS                   //
        // -------------------------------------------------- //

        public string shader { get; set; }

        private List<DecalProperty> m_Properties;
        public List<DecalProperty> properties
        {
            get { return m_Properties; }
        }

        // -------------------------------------------------- //
        //                   PUBLIC METHODS                   //
        // -------------------------------------------------- //

        /// <summary>
        /// Add a DecalProperty.
        /// </summary>
        /// <param name="value">DecalProperty to add.</param>
        public void AddDecalProperty(DecalProperty value)
        {
            if(m_Properties == null)
                m_Properties = new List<DecalProperty>();

            m_Properties.Add(value);
        }
    }
}
