using System;
using UnityEngine;

namespace kTools.Decals
{
    [Serializable]
    public abstract class DecalProperty
    {
        // -------------------------------------------------- //
        //                   PRIVATE FIELDS                   //
        // -------------------------------------------------- //

        [SerializeField] public string displayName;
        [SerializeField] public string referenceName;

        // -------------------------------------------------- //
        //                 ABSTRACT METHODS                   //
        // -------------------------------------------------- //

        /// <summary>
        /// Set the ShaderProperty on a Material.
        /// </summary>
        /// <param name="material">Material to set the ShaderProperty on.</param>
        public abstract void SetProperty(Material material);
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

        // -------------------------------------------------- //
        //                   PUBLIC METHODS                   //
        // -------------------------------------------------- //

        /// <summary>
        /// Set the ShaderProperty on a Material.
        /// </summary>
        /// <param name="material">Material to set the ShaderProperty on.</param>
        public override void SetProperty(Material material)
        {
            material.SetTexture(referenceName, value);
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

        // -------------------------------------------------- //
        //                   PUBLIC METHODS                   //
        // -------------------------------------------------- //

        /// <summary>
        /// Set the ShaderProperty on a Material.
        /// </summary>
        /// <param name="material">Material to set the ShaderProperty on.</param>
        public override void SetProperty(Material material)
        {
            material.SetColor(referenceName, value);
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

        // -------------------------------------------------- //
        //                   PUBLIC METHODS                   //
        // -------------------------------------------------- //

        /// <summary>
        /// Set the ShaderProperty on a Material.
        /// </summary>
        /// <param name="material">Material to set the ShaderProperty on.</param>
        public override void SetProperty(Material material)
        {
            material.SetFloat(referenceName, value);
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

        // -------------------------------------------------- //
        //                   PUBLIC METHODS                   //
        // -------------------------------------------------- //

        /// <summary>
        /// Set the ShaderProperty on a Material.
        /// </summary>
        /// <param name="material">Material to set the ShaderProperty on.</param>
        public override void SetProperty(Material material)
        {
            material.SetVector(referenceName, value);
        }
    }
}
