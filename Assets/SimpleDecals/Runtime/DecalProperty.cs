using System;
using UnityEngine;

namespace kTools.ShaderUtil
{
    [Serializable]
    public abstract class DecalProperty
    {
        // -------------------------------------------------- //
        //                   PRIVATE FIELDS                   //
        // -------------------------------------------------- //

        public string displayName;
        public string referenceName;

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

        public Texture2D value;

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

        public Color value;

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

        public float value;

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

        public Vector4 value;

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
