using System;
using UnityEngine;
using System.Linq;

namespace kTools.Decals
{
    [CreateAssetMenu(fileName = "New ScriptableDecal", menuName = "kTools/ScriptableDecal", order = 1)]
	public sealed class ScriptableDecal : ScriptableObject 
    {
        // -------------------------------------------------- //
        //                   PRIVATE FIELDS                   //
        // -------------------------------------------------- //

        [SerializeField] private string m_DecalDefinitionType;
        public string decalDefinitionType
        {
            get { return m_DecalDefinitionType; }
        }

        [SerializeField] private int m_MaxInstances = 100;
        public int maxInstances
        {
            get { return m_MaxInstances; }
        }

        [SerializeField] private string m_Shader;
        public string shader
        {
            get { return m_Shader; }
        }

        [SerializeField] private SerializableDecalProperty[] m_SerializedProperties;
        public SerializableDecalProperty[] serializedProperties
        {
            get { return m_SerializedProperties; }
        }

        // -------------------------------------------------- //
        //                    CONSTRUCTORS                    //
        // -------------------------------------------------- //

        public ScriptableDecal()
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
            if(value.AssemblyQualifiedName == m_DecalDefinitionType)
                return;
            
            m_DecalDefinitionType = value.AssemblyQualifiedName;

            // Define Decal and serialize
            var definition = (DecalDefinition)Activator.CreateInstance(value);
            DecalDefinitionContext context;
            definition.DefineDecal(out context);
            ConvertContextToSerializableData(context);
        }

        // -------------------------------------------------- //
        //                   PRIVATE METHODS                  //
        // -------------------------------------------------- //

        // Convert a DecalDefinitionContext to serializable data
        private void ConvertContextToSerializableData(DecalDefinitionContext context)
        {
            // Common fields
            m_Shader = context.shader;

            // Serializable Decal properties
            if(context.properties != null)
            {
                m_SerializedProperties = new SerializableDecalProperty[context.properties.Count];
                for(int i = 0; i < m_SerializedProperties.Length; i++)
                {
                    m_SerializedProperties[i] = new SerializableDecalProperty()
                    {
                        displayName = context.properties[i].displayName,
                        referenceName = context.properties[i].referenceName,
                    };
                    if(context.properties[i] as TextureDecalProperty != null)
                    {
                        TextureDecalProperty textureProp = context.properties[i] as TextureDecalProperty;
                        m_SerializedProperties[i].type = PropertyType.Texture;
                        m_SerializedProperties[i].textureValue = textureProp.value;
                    }
                    else if(context.properties[i] as ColorDecalProperty != null)
                    {
                        ColorDecalProperty colorProp = context.properties[i] as ColorDecalProperty;
                        m_SerializedProperties[i].type = PropertyType.Color;
                        m_SerializedProperties[i].colorValue = colorProp.value;
                    }
                    else if(context.properties[i] as FloatDecalProperty != null)
                    {
                        FloatDecalProperty floatProp = context.properties[i] as FloatDecalProperty;
                        m_SerializedProperties[i].type = PropertyType.Float;
                        m_SerializedProperties[i].floatValue = floatProp.value;
                    }
                    else if(context.properties[i] as VectorDecalProperty != null)
                    {
                        VectorDecalProperty vectorProp = context.properties[i] as VectorDecalProperty;
                        m_SerializedProperties[i].type = PropertyType.Vector;
                        m_SerializedProperties[i].vectorValue = vectorProp.value;
                    }
                    else if(context.properties[i] as KeywordDecalProperty != null)
                    {
                        KeywordDecalProperty keywordProp = context.properties[i] as KeywordDecalProperty;
                        m_SerializedProperties[i].type = PropertyType.Keyword;
                        m_SerializedProperties[i].boolValue = keywordProp.value;
                    }
                    else
                        Debug.LogError("Not a valid Property type!");
                }
            }
        }
    }
}
