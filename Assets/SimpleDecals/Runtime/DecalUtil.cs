using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

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

        // -------------------------------------------------- //
        //                  ATTRIBUTE UTILS                   //
        // -------------------------------------------------- //

        // Get the first Attribute of a Type on a certain Type
        public static T GetAttribute<T>(this Type type) where T : Attribute
        {
            Assert.IsTrue(type.IsDefined(typeof(T), false), "Attribute not found");
            return (T)type.GetCustomAttributes(typeof(T), false)[0];
        }
    }
}
