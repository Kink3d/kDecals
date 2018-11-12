using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace kTools.Decals
{
    [ExecuteInEditMode]
    public static class DecalUtil
    {
        // -------------------------------------------------- //
        //                   GEOMETRY UTILS                   //
        // -------------------------------------------------- //
        
        /// <summary>
        /// Get a direction vector from a Decal to the nearest Collider face.
        /// </summary>
        /// <param name="decal">Decal to use as vector source.</param>
        public static Vector3 GetDirectionToNearestFace(Decal decal)
        {
            Vector3 hitPoint = Vector3.zero;
            return GetDirectionToNearestFace(decal, out hitPoint);
        }

        /// <summary>
        /// Get a direction vector from a Decal to the nearest Collider face.
        /// </summary>
        /// <param name="decal">Decal to use as vector source.</param>
        /// <param name="hitPoint">The location of the hit on the Collider face.</param>
        public static Vector3 GetDirectionToNearestFace(Decal decal, out Vector3 hitPoint)
        {
            Vector3 nearestVector = Vector3.zero;
            float nearestDistance = Mathf.Infinity;
            hitPoint = Vector3.zero;

            for(int i = 0; i < 1000; i++)
            {
                Vector3 direction = RandomSphericalDistributionVector();
                RaycastHit hit;
                if(Physics.Raycast(decal.transform.position, direction, out hit, 1000))
                {
                    float distance = Vector3.Distance(hit.point, decal.transform.position);
                    if(distance < nearestDistance)
                    {
                        nearestVector = direction;
                        nearestDistance = distance;
                        hitPoint = hit.point;
                    }
                }
            }
            return nearestVector;
        }

        // Get a random direction vector based on uniform spherical distribution
        private static Vector3 RandomSphericalDistributionVector()
        {
            float theta = UnityEngine.Random.Range(-(Mathf.PI / 2), Mathf.PI / 2);
            float phi = UnityEngine.Random.Range(0, Mathf.PI * 2);

            float x = Mathf.Cos(theta) * Mathf.Cos(phi);
            float y = Mathf.Cos(theta) * Mathf.Sin(phi);
            float z = Mathf.Sin(theta);

            return new Vector3(x, y, z);
        }

        // -------------------------------------------------- //
        //                    OBJECT UTILS                    //
        // -------------------------------------------------- //

        /// <summary>
        /// Safely destroy an Object.
        /// </summary>
        /// <param name="obj">Object to destroy.</param>
        public static void Destroy(UnityEngine.Object obj)
        {
            #if UNITY_EDITOR
                UnityEngine.Object.DestroyImmediate(obj);
            #else
                UnityEngine.Destroy(obj);
            #endif
        }

        // -------------------------------------------------- //
        //                     TYPE UTILS                     //
        // -------------------------------------------------- //

        /// <summary>
        /// Get all Types in the current Assembly that are subclass of a Type.
        /// </summary>
        /// <param name="parentType">Type to return children of.</param>
        public static IEnumerable<Type> GetAllAssemblySubclassTypes(Type parentType)
        {
            return GetAllAssemblyTypes()
                .Where(
                    t => t.IsSubclassOf(parentType)
                    && !t.IsAbstract
                    );
        }

        /// <summary>
        /// Get all Types in current Assembly.
        /// </summary>
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

        /// <summary>
        /// Get the first Attribute of a Type on a certain Type.
        /// </summary>
        public static T GetAttribute<T>(this Type type) where T : Attribute
        {
            Assert.IsTrue(type.IsDefined(typeof(T), false), "Attribute not found");
            return (T)type.GetCustomAttributes(typeof(T), false)[0];
        }
    }
}
