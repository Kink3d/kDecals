using System;
using System.Collections.Generic;
using UnityEngine;
using kTools.ShaderUtil;

namespace kTools.Decals
{
    [Serializable]
    public struct DecalDefinitionContext
    {
        // -------------------------------------------------- //
        //                   PRIVATE FIELDS                   //
        // -------------------------------------------------- //

        public string name { get; set; }
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
        /// Add a ShaderProperty.
        /// </summary>
        /// <param name="value">ShaderProperty to add.</param>
        public void AddShaderProperty(DecalProperty value)
        {
            if(m_Properties == null)
                m_Properties = new List<DecalProperty>();

            m_Properties.Add(value);
        }
    }
}
