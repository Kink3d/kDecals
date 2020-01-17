using System;

namespace kTools.Decals
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class DecalDefinitionAttribute : Attribute
    {
        // The location for the DecalDefinition in the Select menu
        public readonly string menuItem;

        // -------------------------------------------------- //
        //                    CONSTRUCTORS                    //
        // -------------------------------------------------- //
        
        internal DecalDefinitionAttribute(string menuItem)
        {
            this.menuItem = menuItem;
        }
    }
}
