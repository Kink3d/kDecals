using System;
using UnityEngine;

namespace kTools.ShaderUtil
{
    [Serializable]
    public abstract class ShaderProperty
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
    public class TextureProperty : ShaderProperty
    {
        // -------------------------------------------------- //
        //                   PRIVATE FIELDS                   //
        // -------------------------------------------------- //

        public Texture2D value;

        // -------------------------------------------------- //
        //                    CONSTRUCTORS                    //
        // -------------------------------------------------- //

        public TextureProperty(string displayName, string referenceName, Texture2D value)
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
    public class ColorProperty : ShaderProperty
    {
        // -------------------------------------------------- //
        //                   PRIVATE FIELDS                   //
        // -------------------------------------------------- //

        public Color value;

        // -------------------------------------------------- //
        //                    CONSTRUCTORS                    //
        // -------------------------------------------------- //

        public ColorProperty(string displayName, string referenceName, Color value)
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
    public class FloatProperty : ShaderProperty
    {
        // -------------------------------------------------- //
        //                   PRIVATE FIELDS                   //
        // -------------------------------------------------- //

        public float value;

        // -------------------------------------------------- //
        //                    CONSTRUCTORS                    //
        // -------------------------------------------------- //

        public FloatProperty(string displayName, string referenceName, float value)
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
    public class VectorProperty : ShaderProperty
    {
        // -------------------------------------------------- //
        //                   PRIVATE FIELDS                   //
        // -------------------------------------------------- //

        public Vector4 value;

        // -------------------------------------------------- //
        //                    CONSTRUCTORS                    //
        // -------------------------------------------------- //

        public VectorProperty(string displayName, string referenceName, Vector4 value)
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
