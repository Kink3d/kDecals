using System;

namespace kTools.Decals
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class DecalDefinitionAttribute : Attribute
    {
        public readonly string menuItem;

        internal DecalDefinitionAttribute(string menuItem)
        {
            this.menuItem = menuItem;
        }
    }
}
