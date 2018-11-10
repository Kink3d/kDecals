using UnityEngine;

namespace kTools.Decals
{
    public static class DecalSystem
    {
        /// <summary>
        /// Get a Decal instance.
        /// </summary>
        /// <param name="positionWS">Decal position in World space.</param>
        /// <param name="directionWS">World space direction/normal vector to use for Decal rotation.</param>
        /// <param name="decalData">DecalData to set.</param>
        /// <param name="usePooling">If true the Decal will be taken from a DecalPooler instance.</param>
        public static kDecal GetDecal(Vector3 positionWS, Vector3 directionWS, kDecalData decalData, bool usePooling)
        {
			return GetDecal(positionWS, directionWS, Vector2.one, decalData, usePooling);
        }

        /// <summary>
        /// Get a Decal instance.
        /// </summary>
        /// <param name="positionWS">Decal position in World space.</param>
        /// <param name="directionWS">World space direction/normal vector to use for Decal rotation.</param>
        /// <param name="scaleWS">Decal scale in World space.</param>
        /// <param name="decalData">DecalData to set.</param>
        /// <param name="usePooling">If true the Decal will be taken from a DecalPooler instance.</param>
		public static kDecal GetDecal(Vector3 positionWS, Vector3 directionWS, Vector2 scaleWS, kDecalData decalData, bool usePooling)
        {
            kDecal decal = GetDecalInstance(decalData, usePooling);
            decal.SetDecalActive(true);
            decal.SetDecalTransform(positionWS, directionWS, scaleWS);
            decal.SetDecalData(decalData);
			return decal;
        }

        /// <summary>
        /// Get a Decal instance.
        /// </summary>
        /// <param name="positionWS">Decal position in World space.</param>
        /// <param name="rotationWS">Decal rotation in World space.</param>
        /// <param name="decalData">DecalData to set.</param>
        /// <param name="usePooling">If true the Decal will be taken from a DecalPooler instance.</param>
        public static kDecal GetDecal(Vector3 positionWS, Quaternion rotationWS, kDecalData decalData, bool usePooling)
        {
			return GetDecal(positionWS, rotationWS, Vector2.one, decalData, usePooling);
        }

        /// <summary>
        /// Get a Decal instance.
        /// </summary>
        /// <param name="positionWS">Decal position in World space.</param>
        /// <param name="rotationWS">Decal rotation in World space.</param>
        /// <param name="scaleWS">Decal scale in World space.</param>
        /// <param name="decalData">DecalData to set.</param>
        /// <param name="usePooling">If true the Decal will be taken from a DecalPooler instance.</param>
		public static kDecal GetDecal(Vector3 positionWS, Quaternion rotationWS, Vector2 scaleWS, kDecalData decalData, bool usePooling)
        {
            kDecal decal = GetDecalInstance(decalData, usePooling);
            decal.SetDecalActive(true);
            decal.SetDecalTransform(positionWS, rotationWS, scaleWS);
            decal.SetDecalData(decalData);
			return decal;
        }

        /// <summary>
        /// Create a new Decal directly.
        /// </summary>
        /// <param name="decalData">DecalData to set.</param>
        public static kDecal CreateDecal(kDecalData decalData)
        {
            GameObject obj = new GameObject();
            obj.name = string.Format("Decal_{0}", decalData.name);
            Transform transform = obj.transform;
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            return transform.gameObject.AddComponent<kDecal>();
        }

        // -------------------------------------------------- //
        //                  INTERNAL METHODS                  //
        // -------------------------------------------------- //

        // Get a Decal instance, either from pooling or directly
        private static kDecal GetDecalInstance(kDecalData decalData, bool usePooling)
        {
            if(usePooling)
            {
                // Ensure there is an active DecalPooler
                DecalPooler decalPooler = GameObject.FindObjectOfType<DecalPooler>();
                if(decalPooler == null)
                    CreateDecalPooler();

                // Get a poolable Decal instance
                kDecal decal;
                DecalPooler.Instance.TryGetInstance(decalData, out decal);
                return decal;
            }
            else
                // Create single Decal
                return CreateDecal(decalData);
        }

        // Create a new DecalPooler instance
        private static void CreateDecalPooler()
        {
            GameObject obj = new GameObject();
            obj.name = "DecalPooler";
            obj.AddComponent<DecalPooler>();
        }
    }
}
