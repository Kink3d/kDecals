using UnityEngine;

namespace kTools.Decals
{
    [ExecuteInEditMode]
    [AddComponentMenu("kTools/Decal")]
    [RequireComponent(typeof(Projector))]
    public sealed class Decal : MonoBehaviour
    {
        // -------------------------------------------------- //
        //                        ENUM                        //
        // -------------------------------------------------- //

        public enum Axis { PositiveX, NegativeX, PositiveY, NegativeY, PositiveZ, NegativeZ }

        // -------------------------------------------------- //
        //                   PRIVATE FIELDS                   //
        // -------------------------------------------------- //

        private static int m_AxisPropertyID = Shader.PropertyToID("_Axis");

        private Vector3 m_PreviousScale = Vector3.one;

        private Projector m_Projector;
		public Projector projector
		{
			get 
			{
				if(m_Projector == null)
					m_Projector = GetComponent<Projector>();
				return m_Projector;
			}
		}

        [SerializeField] private ScriptableDecal m_DecalData;
        public ScriptableDecal decalData
        {
            get { return m_DecalData; }
        }

        [SerializeField] private Material m_Material;
        public Material material
        {
            get { return m_Material; }
            set
            {
                if(m_Material)
                    DecalUtil.Destroy(m_Material);
                m_Material = value;
            }
        }

        // -------------------------------------------------- //
        //                ENGINE LOOP METHODS                 //
        // -------------------------------------------------- //

        private void OnEnable()
		{
#if UNITY_EDITOR
            // Collapse Projector UI as user shouldnt edit it
			UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(projector, false);
#endif
            // Initialize the Projector
            SetupProjector();
		}

        private void Update()
        {
            // Track Decal scale so it can be edited from the Transform
            if(transform.localScale != m_PreviousScale)
            {
                m_PreviousScale = transform.lossyScale;
                SetDecalScale(new Vector2(m_PreviousScale.x, m_PreviousScale.y));
            }
        }

        // -------------------------------------------------- //
        //                   PUBLIC METHODS                   //
        // -------------------------------------------------- //

        /// <summary>
        /// Refresh the Decal's renderer.
        /// </summary>
        public void Refresh()
        {
            RefreshInternal();
        }

        /// <summary>
        /// Activates/Deactivates the Decal.
        /// </summary>
        /// <param name="value">Activate or deactivate the Decal.</param>
        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

        /// <summary>
        /// Set DecalData for the Decal and refresh its renderer.
        /// </summary>
        /// <param name="value">DecalData to set.</param>
        public void SetData(ScriptableDecal value)
        {
            m_DecalData = value;
            RefreshInternal();
        }

        /// <summary>
        /// Sets a full Decal transform.
        /// </summary>
        /// <param name="positionWS">Decal position in World space.</param>
        /// <param name="rotationWS">Decal rotation in World space.</param>
        /// <param name="scaleWS">Decal scale in World space.</param>
        public void SetTransform(Vector3 positionWS, Quaternion rotationWS, Vector2 scaleWS)
        {
            SetDecalPosition(positionWS);
            SetDecalRotation(rotationWS);
            SetDecalScale(scaleWS);
        }

        /// <summary>
        /// Sets a full Decal transform (using a direction vector for rotation).
        /// </summary>
        /// <param name="positionWS">Decal position in World space.</param>
        /// <param name="directionWS">World space direction/normal vector to use for Decal rotation.</param>
        /// <param name="scaleWS">Decal scale in World space.</param>
        /// <param name="randomRotationZ">If true Decal will be randomly rotated on its local Z axis.</param>
        public void SetTransform(Vector3 positionWS, Vector3 directionWS, Vector2 scaleWS, bool randomRotationZ = false)
        {
            SetDecalPosition(positionWS);
            SetDecalRotation(directionWS, randomRotationZ);
            SetDecalScale(scaleWS);
        }

        // -------------------------------------------------- //
        //                  INTERNAL METHODS                  //
        // -------------------------------------------------- //

        // Initialize Projector state
        private void SetupProjector()
        {
			projector.orthographic = true;
			projector.nearClipPlane = -0.5f;
			projector.farClipPlane = 0.5f;
        }

        // Set Decal position in World space
        private void SetDecalPosition(Vector3 positionWS)
        {
            transform.position = positionWS;
        }

        // Set Decal rotation based on a direction vector
        private void SetDecalRotation(Quaternion rotationWS)
		{
            transform.rotation = rotationWS;
		}

        // Set Decal rotation based on a direction vector
        private void SetDecalRotation(Vector3 directionWS, bool randomRotationZ = false)
		{
            var axis = GetAxisFromDirection(directionWS);
			var randomZ = randomRotationZ ? UnityEngine.Random.Range(0f, 360f) : 0f;
            var rotation = Vector3.zero;
			switch(axis)
			{
				case Axis.NegativeX:
					rotation = new Vector3(0, 90, randomZ);
                    break;
				case Axis.PositiveX:
					rotation = new Vector3(0, -90, randomZ);
                    break;
				case Axis.NegativeY:
					rotation = new Vector3(90, 0, randomZ);
                    break;
				case Axis.PositiveY:
					rotation = new Vector3(-90, 0, randomZ);
                    break;
				case Axis.NegativeZ:
					rotation = new Vector3(0, 0, randomZ);
                    break;
				default: //Axis.PositiveZ
					rotation = new Vector3(0, 180, randomZ);
                    break;
			}
            transform.eulerAngles = rotation;
		}

        // Set Decal scale directly on the Projector
        private void SetDecalScale(Vector2 scaleWS)
        {
			projector.orthographicSize = scaleWS.y * 0.5f;
			projector.aspectRatio = scaleWS.x / scaleWS.y;
        }

        // Get a projection axis from a direction vector
        private Axis GetAxisFromDirection(Vector3 directionWS)
		{
			if(Mathf.Abs(directionWS.x) > 0.5)
				return directionWS.x > 0 ? Axis.NegativeX : Axis.PositiveX;
			else if(Mathf.Abs(directionWS.y) > 0.5)
				return directionWS.y < 0 ? Axis.NegativeY : Axis.PositiveY;
			else //if(Mathf.Abs(direction.z) > 0.5)
				return directionWS.z > 0 ? Axis.NegativeZ : Axis.PositiveZ;
		}

        // Set all Material values on the Decal
        private void RefreshInternal()
        {
            // If DecalData hasnt been initialized correctly
            if(m_DecalData == null || m_DecalData.shader == null)
            {
                Debug.LogError("DecalData is not fully defined for this Decal. Aborting.");
                return;
            }

            // Calculate correct relative direction vector
            var direction = transform.forward;

            // TODO
            // - Rewrite this section with PropetyBlock after Projector removal

            // Initialize material and set common properties
			material = new Material(Shader.Find(m_DecalData.shader));
			material.SetInt(m_AxisPropertyID, (int)GetAxisFromDirection(direction));

            // Set properties from DecalDefinition
            foreach(SerializableDecalProperty prop in decalData.serializedProperties)
                prop.SetProperty(material);

			projector.material = material;
        }
        
        // -------------------------------------------------- //
        //                       GIZMOS                       //
        // -------------------------------------------------- //

        void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "kTools/Decals/Decal icon.png", true);
        }
    }
}
