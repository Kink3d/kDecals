using System.Collections.Generic;
using UnityEngine;
using kTools.Pooling;

namespace kTools.Decals
{
    /// <summary>
    /// kPooling Processor for Decals.
    /// </summary>
    public sealed class DecalProcessor : Processor<Decal>
    {
#region Fields
        Dictionary<object, Transform> m_Containers;
#endregion

#region Constructors
        public DecalProcessor()
        {
            // Set data
            m_Containers = new Dictionary<object, Transform>();
        }
#endregion

#region Overrides
        public override Decal CreateInstance(object key, Decal source)
        {
            // Find container Transform matching key
            if(!m_Containers.TryGetValue(key, out _))
            {
                // No matching container Transform so create one
                Transform container = new GameObject($"DecalPool - {source.name}").transform;
                m_Containers.Add(key, container);
            }

            // Create Instance
            var obj = GameObject.Instantiate(source, Vector3.zero, Quaternion.identity);
            obj.name = source.name;
            OnDisableInstance(key, obj);
            return obj;
        }

        public override void DestroyInstance(object key, Decal instance)
        {
            // Destroy instance
            DestroyGameObject(instance.gameObject);

            // Find container Transform matching key
            Transform container;
            if(m_Containers.TryGetValue(key, out container))
            {
                // Last instance so destroy container Transform
                if(container.childCount == 0)
                {
                    DestroyGameObject(container.gameObject);
                    m_Containers.Remove(key);
                }
            }
        }

        public override void OnEnableInstance(object key, Decal instance)
        {
            // Parenting
            instance.transform.SetParent(null);

            // Active
            instance.gameObject.SetActive(true);
        }

        public override void OnDisableInstance(object key, Decal instance)
        {
            // Parenting
            Transform container;
            if(m_Containers.TryGetValue(key, out container))
            {
                instance.transform.SetParent(container);
            }

            // Active
            instance.gameObject.SetActive(false);

            // Transform
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.localScale = Vector3.one;
        }
#endregion

#region GameObject
        void DestroyGameObject(GameObject gameObject)
        {
            #if UNITY_EDITOR
            Object.DestroyImmediate(gameObject);
            #else
            Object.Destroy(gameObject);
            #endif
        }
#endregion
    }
}
