using UnityEngine;

namespace kTools.Decals
{
    public enum DecalType
    {
        Projection,
        Mesh
    }
    
    /// <summary>
    /// Decal Object component.
    /// </summary>
    [AddComponentMenu("kTools/Decal"), ExecuteInEditMode]
    public sealed class Decal : MonoBehaviour
    {
        static Vector3[] s_VectorPlanes = new Vector3[8];

#region Serialized Fields
        [SerializeField]
        DecalType m_DecalType;

        [SerializeField]
        DecalData m_DecalData;

        [SerializeField]
        Mesh m_Mesh;

        [SerializeField]
        int m_SubmeshIndex = 0;
#endregion

#region Fields
        const string kGizmoPath = "Packages/com.kink3d.decals/Gizmos/Decal.png";
        const float kDefaultScale = 0.5f;

        Matrix4x4 m_Matrix;
        Plane[] m_ClipPlanes;
        DecalData m_PreviousDecalData;
#endregion

#region Constructors
        public Decal()
        {
            m_ClipPlanes = new Plane[6];
        }
#endregion

#region Properties
        /// <summary>Decal Type defines how this Decal should be rendered.</summary>
        public DecalType decalType
        {
            get => m_DecalType;
            set => m_DecalType = value;
        }

        /// <summary>Data object providing settings and inputs for this Decal.</summary>
        public DecalData decalData
        {
            get => m_DecalData;
            set => m_DecalData = value;
        }

        /// <summary>Mesh for Decal.</summary>
        public Mesh mesh
        {
            get => m_Mesh;
            set => m_Mesh = value;
        }

        /// <summary>Submesh index for Mesh Decals.</summary>
        public int subMeshIndex
        {
            get => m_SubmeshIndex;
            set => m_SubmeshIndex = value;
        }

        /// <summary>Decal projection matrix.</summary>
        public Matrix4x4 matrix => m_Matrix;

        /// <summary>Clipping planes used for culling projection Decals.</summary>
        public Plane[] clipPlanes => m_ClipPlanes;
#endregion

#region State
        void OnEnable()
        {
            // Registration
            DecalSystem.RegisterDecal(this);
        }

        void OnDisable()
        {
            // Registration
            DecalSystem.UnregisterDecal(this);
        }

        void Update()
        {
            if(decalData == null)
                return;
            
            if(decalType == DecalType.Projection)
            {
                UpdateProjectionDecal();
            }
        }

        bool HasDecalDataChanged()
        {
            if(decalData != m_PreviousDecalData)
            {
                m_PreviousDecalData = decalData;
                return true;
            }

            return false;
        }

        bool HasDepthChanged()
        {
            return decalData.depth != transform.localScale.z;
        }
#endregion

#region Transform
        /// <summary>
        /// Set Transform data for Decal.
        /// </summary>
        /// <param name="position">World space position for Decal.</param>
        /// <param name="direction">World space forward direction for Decal.</param>
        /// <param name="scale">Local space scale for Decal.</param>
        public void SetTransform(Vector3 position, Vector3 direction, Vector2 scale)
        {
            // Setup
            var depth = decalData != null ? decalData.depth : 1;

            // Set Transform values
            transform.position = position;
            transform.LookAt(transform.position + direction.normalized);
            transform.localScale = new Vector3(scale.x, scale.y, depth);
        }
#endregion

#region Projection
        void UpdateProjectionDecal()
        {
            if(HasDepthChanged())
            {
                var localScale = transform.localScale;
                transform.localScale = new Vector3(localScale.x, localScale.y, decalData.depth);
            }

            if(HasDecalDataChanged() || transform.hasChanged)
            {
                UpdateProjectionMatrix();
                UpdateCullingPlanes();
            }
        }

        void UpdateProjectionMatrix()
        {
            // Setup
            var nearClip = Mathf.Max(transform.localScale.z, Mathf.Epsilon);
            var farClip = 0.0f;

            // Get Matrix
            m_Matrix = Matrix4x4.Ortho(-kDefaultScale, kDefaultScale, -kDefaultScale, kDefaultScale, nearClip, farClip);
            
            // Offset
            m_Matrix.m02 += 0.5f * m_Matrix.m32;
            m_Matrix.m03 += 0.5f * m_Matrix.m33;
            m_Matrix.m12 += 0.5f * m_Matrix.m32;
            m_Matrix.m13 += 0.5f * m_Matrix.m33;

            // Scaling
            var zScale = 1.0f / (farClip - nearClip);
            m_Matrix.m00 *= 0.5f;
            m_Matrix.m11 *= 0.5f;
            m_Matrix.m22 = zScale * transform.localScale.z;
            m_Matrix.m23 = -zScale * transform.localScale.z;

            // Transformation
            m_Matrix = m_Matrix * transform.worldToLocalMatrix;
        }

        void UpdateCullingPlanes()
        {
            // Create plane vertices
            var vertices = s_VectorPlanes;
            vertices[0].x = vertices[1].x = vertices[4].x = vertices[5].x = -kDefaultScale;
            vertices[2].x = vertices[3].x = vertices[6].x = vertices[7].x = kDefaultScale;
            vertices[0].y = vertices[2].y = vertices[4].y = vertices[6].y = kDefaultScale;
            vertices[1].y = vertices[3].y = vertices[5].y = vertices[7].y = -kDefaultScale;
			vertices[0].z = vertices[1].z = vertices[2].z = vertices[3].z = transform.localScale.z;
			vertices[7].z = vertices[4].z = vertices[6].z = vertices[5].z = 0.0f;

            // Transform vertices to World Space
			for (int i = 0; i < 8; i++)
			{
				vertices[i] = transform.TransformPoint(vertices[i]);
			}

            // Create clip planes
            m_ClipPlanes[0] = new Plane(vertices[0], vertices[1], vertices[4]);
            m_ClipPlanes[1] = new Plane(vertices[3], vertices[2], vertices[7]);
            m_ClipPlanes[2] = new Plane(vertices[2], vertices[0], vertices[6]);
            m_ClipPlanes[3] = new Plane(vertices[1], vertices[3], vertices[5]);
            m_ClipPlanes[4] = new Plane(vertices[0], vertices[2], vertices[1]);
            m_ClipPlanes[5] = new Plane(vertices[4], vertices[5], vertices[6]);
        }
#endregion

#region AssetMenu
#if UNITY_EDITOR
        // Add a menu item to Decals
        [UnityEditor.MenuItem("GameObject/kTools/Decal", false, 10)]
        static void CreateDecalObject(UnityEditor.MenuCommand menuCommand)
        {
            // Create Decal
            GameObject go = new GameObject("New Decal", typeof(Decal));
            
            // Transform
            UnityEditor.GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            
            // Undo and Selection
            UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            UnityEditor.Selection.activeObject = go;
        }
#endif
#endregion

#region Gizmos
#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, kGizmoPath, true);

            switch(decalType)
            {
                case DecalType.Projection:
                    DrawProjectionGizmos();
                    break;
                case DecalType.Mesh:
                    DrawMeshGizmos();
                    break;
                default:
                    throw new System.Exception($"Unknown DecalType ({decalType})");
            }
        }

        void DrawProjectionGizmos()
        {
            var isSelected = UnityEditor.Selection.activeObject == gameObject;
            if (!isSelected)
                return;

            // Setup
            var bounds = new Vector3(1.0f, 1.0f, 0.0f);
            var color = new Color32(0, 120, 255, 255);

            // Draw Gizmos
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = color;
            Gizmos.DrawWireCube(Vector3.zero, bounds);
        }

        void DrawMeshGizmos()
        {
            // TODO: Draw Mesh Gizmos
        }
#endif
#endregion
    }
}
