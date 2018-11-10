using System;
using System.Collections.Generic;
using UnityEngine;
using kTools.ShaderUtil;

namespace kTools.Decals
{
    [Serializable]
    public abstract class DecalDefinition
    {
        // -------------------------------------------------- //
        //                   PRIVATE FIELDS                   //
        // -------------------------------------------------- //

        [SerializeField] private string m_Name;
        public string name
        {
            get { return m_Name; }
        }

        [SerializeField] private string m_Shader;
        public string shader
        {
            get { return m_Shader; }
        }

        [SerializeField] private DecalProperty[] m_ShaderProprties;
        public DecalProperty[] shaderProperties
        {
            get { return m_ShaderProprties; }
        }

        // -------------------------------------------------- //
        //                    CONSTRUCTORS                    //
        // -------------------------------------------------- //

        public DecalDefinition()
        {
            // Define Decal and serialize
            DecalDefinitionContext context;
            DefineDecal(out context);
            ConvertContextToDefinition(context);
        }

        // -------------------------------------------------- //
        //                   PRIVATE METHODS                  //
        // -------------------------------------------------- //

        // Convert a DecalDefinitionContext to Definition values
        private void ConvertContextToDefinition(DecalDefinitionContext context)
        {
            // Common fields
            m_Name = context.name;
            m_Shader = context.shader;

            // Shader properties
            if(context.properties != null)
            {
                m_ShaderProprties = new DecalProperty[context.properties.Count];
                for(int i = 0; i < m_ShaderProprties.Length; i++)
                    m_ShaderProprties[i] = context.properties[i];
            }
        }

        // -------------------------------------------------- //
        //                 ABSTRACT METHODS                   //
        // -------------------------------------------------- //

        /// <summary>
        /// Define the Decal.
        /// </summary>
        /// <param name="context">Context for setting DecalDefinition values.</param>
        public abstract void DefineDecal(out DecalDefinitionContext context);
    }
}
