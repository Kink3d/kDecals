using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace kTools.Decals
{
    public static class DecalUtil
    {
        // -------------------------------------------------- //
        //                     TYPE UTILS                     //
        // -------------------------------------------------- //

        // Get all Types in the current Assembly that are subclass of the Type parentType
        public static IEnumerable<Type> GetAllAssemblySubclassTypes(Type parentType)
        {
            return GetAllAssemblyTypes()
                .Where(
                    t => t.IsSubclassOf(parentType)
                    && !t.IsAbstract
                    );
        }

        // Get all Types in current Assembly
        public static IEnumerable<Type> GetAllAssemblyTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(t =>
                {
                    // Ugly hack to handle mis-versioned dlls
                    var innerTypes = new Type[0];
                    try
                    {
                        innerTypes = t.GetTypes();
                    }
                    catch {}
                    return innerTypes;
                });
        }
    }
}
