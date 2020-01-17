using System;
using System.Collections.Generic;
using UnityEngine;

namespace kTools.Decals
{
    [Serializable]
    public abstract class DecalDefinition
    {
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
