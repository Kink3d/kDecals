using UnityEngine;

namespace kTools.Decals
{
    /// <summary>
    /// Decal Object component.
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("kTools/Decal")]
    [RequireComponent(typeof(Projector))]
    public sealed class Decal : MonoBehaviour
    {
#region Serialized Fields
        [SerializeField]
        DecalData m_DecalData;
#endregion

#region Fields
        const string kGizmoPath = "Packages/com.kink3d.decals/Gizmos/Decal.png";
        const float kScaleMultiplier = 0.5f;

        Projector m_Projector;
        Vector3 m_PreviousScale;
#endregion

#region Properties
        public DecalData decalData => m_DecalData;

        public Projector projector
		{
			get 
			{
				if(m_Projector == null)
					m_Projector = GetComponent<Projector>();
				return m_Projector;
			}
		}
#endregion

#region Initialization
        /// <summary>
        /// Initializes Decal with given DecalData.
        /// </summary>
        /// <param name="decalData">DecalData to set.</param>
        public void Init(DecalData decalData)
        {
            // Set data
            m_DecalData = decalData;
            m_PreviousScale = Vector3.one;

            // Collapse Projector Inspector
            #if UNITY_EDITOR
			UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(projector, false);
            #endif

            // Setup Projector
            projector.material = decalData.material;
            projector.orthographic = true;
			projector.nearClipPlane = Mathf.Max(decalData.projectionDepth, 0.01f);
			projector.farClipPlane = 0.0f;
            projector.orthographicSize = 1.0f * kScaleMultiplier;
            projector.aspectRatio = 1.0f;
        }
#endregion

#region State
        void Update()
        {
            // Capture scale changes
            if(transform.lossyScale != m_PreviousScale)
            {
                // Scale Projector
                var scale = transform.lossyScale;
                projector.orthographicSize = scale.y * kScaleMultiplier;
			    projector.aspectRatio = scale.x / scale.y;

                // Track previous scale
                m_PreviousScale = scale;
            }
        }
#endregion

#region Transform
        /// <summary>
        /// Set Transform data for Decal.
        /// </summary>
        /// <param name="position">World space position for Decal.</param>
        /// <param name="direction">World space forward direction for Decal.</param>
        /// <param name="scale">Local space scale for Decal.</param>
        public void SetTransform(Vector3 position, Vector3 direction, Vector3 scale)
        {
            // Set Transform values
            transform.position = position;
            transform.LookAt(-direction);
            transform.localScale = scale;
        }
#endregion

#region Gizmos
#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            // Setup
            var bounds = new Vector3(1.0f, 1.0f, 0.0f);
            var color = new Color32(0, 120, 255, 255);
            var selectedColor = new Color32(255, 255, 255, 255);
            var isSelected = UnityEditor.Selection.activeObject == gameObject;

            // Draw Gizmos
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = isSelected ? selectedColor : color;
            Gizmos.DrawIcon(transform.position, kGizmoPath, true);
            Gizmos.DrawWireCube(Vector3.zero, bounds);
        }
#endif
#endregion
    }
}
