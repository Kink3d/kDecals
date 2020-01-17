using System;
using UnityEngine;

namespace kTools.Decals
{
    // -------------------------------------------------- //
    //                        ENUMS                       //
    // -------------------------------------------------- //

    [Serializable] public enum PropertyType { Texture, Color, Float, Vector, Keyword }

    [Serializable]
    public class SerializableDecalProperty
    {
        // -------------------------------------------------- //
        //                    PUBLIC FIELDS                   //
        // -------------------------------------------------- //

        [SerializeField] public string displayName;
        [SerializeField] public string referenceName;
        [SerializeField] public PropertyType type;
        [SerializeField] public Texture2D textureValue;
        [SerializeField] public Color colorValue;
        [SerializeField] public float floatValue;
        [SerializeField] public Vector4 vectorValue;
        [SerializeField] public bool boolValue;

        // -------------------------------------------------- //
        //                   PUBLIC METHODS                   //
        // -------------------------------------------------- //

        /// <summary>
        /// Set the SerializedDecalProperty on a Material.
        /// </summary>
        /// <param name="material">Material to set the SerializedDecalProperty on.</param>
        public void SetProperty(Material material)
        {
            switch(type)
            {
                case PropertyType.Texture:
                    material.SetTexture(referenceName, textureValue);
                    break;
                case PropertyType.Color:
                    material.SetColor(referenceName, colorValue);
                    break;
                case PropertyType.Float:
                    material.SetFloat(referenceName, floatValue);
                    break;
                case PropertyType.Vector:
                    material.SetVector(referenceName, vectorValue);
                    break;
                case PropertyType.Keyword:
                    if(boolValue == true)
                        material.EnableKeyword(referenceName);
                    else
                        material.DisableKeyword(referenceName);
                    break;
                default:
                    Debug.LogError("Not a valid Property type!");
                    break;
            }
        }
    }

    [Serializable]
    public abstract class DecalProperty
    {
        // -------------------------------------------------- //
        //                   PRIVATE FIELDS                   //
        // -------------------------------------------------- //

        [SerializeField] public string displayName;
        [SerializeField] public string referenceName;
    }

    [Serializable]
    public class TextureDecalProperty : DecalProperty
    {
        // -------------------------------------------------- //
        //                   PRIVATE FIELDS                   //
        // -------------------------------------------------- //

        [SerializeField] public Texture2D value;

        // -------------------------------------------------- //
        //                    CONSTRUCTORS                    //
        // -------------------------------------------------- //

        public TextureDecalProperty(string displayName, string referenceName, Texture2D value)
        {
            this.displayName = displayName;
            this.referenceName = referenceName;
            this.value = value;
        }
    }

    [Serializable]
    public class ColorDecalProperty : DecalProperty
    {
        // -------------------------------------------------- //
        //                   PRIVATE FIELDS                   //
        // -------------------------------------------------- //

        [SerializeField] public Color value;

        // -------------------------------------------------- //
        //                    CONSTRUCTORS                    //
        // -------------------------------------------------- //

        public ColorDecalProperty(string displayName, string referenceName, Color value)
        {
            this.displayName = displayName;
            this.referenceName = referenceName;
            this.value = value;
        }
    }

    [Serializable]
    public class FloatDecalProperty : DecalProperty
    {
        // -------------------------------------------------- //
        //                   PRIVATE FIELDS                   //
        // -------------------------------------------------- //

        [SerializeField] public float value;

        // -------------------------------------------------- //
        //                    CONSTRUCTORS                    //
        // -------------------------------------------------- //

        public FloatDecalProperty(string displayName, string referenceName, float value)
        {
            this.displayName = displayName;
            this.referenceName = referenceName;
            this.value = value;
        }
    }

    [Serializable]
    public class VectorDecalProperty : DecalProperty
    {
        // -------------------------------------------------- //
        //                   PRIVATE FIELDS                   //
        // -------------------------------------------------- //

        [SerializeField] public Vector4 value;

        // -------------------------------------------------- //
        //                    CONSTRUCTORS                    //
        // -------------------------------------------------- //

        public VectorDecalProperty(string displayName, string referenceName, Vector4 value)
        {
            this.displayName = displayName;
            this.referenceName = referenceName;
            this.value = value;
        }
    }

    [Serializable]
    public class KeywordDecalProperty : DecalProperty
    {
        // -------------------------------------------------- //
        //                   PRIVATE FIELDS                   //
        // -------------------------------------------------- //

        [SerializeField] public bool value;

        // -------------------------------------------------- //
        //                    CONSTRUCTORS                    //
        // -------------------------------------------------- //

        public KeywordDecalProperty(string displayName, string referenceName, bool value)
        {
            this.displayName = displayName;
            this.referenceName = referenceName;
            this.value = value;
        }
    }
}
