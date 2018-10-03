using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleTools.Decals
{
	public enum BlendMode { Multiply }
	public enum Axis { PositiveX, NegativeX, PositiveY, NegativeY, PositiveZ, NegativeZ }

	[AddComponentMenu("")]
	[RequireComponent(typeof(Projector))]
	public class Decal : MonoBehaviour 
	{
		private static Dictionary<BlendMode, string> s_ShaderFromBlendMode = new Dictionary<BlendMode, string>()
		{
			{ BlendMode.Multiply, "Hidden/SimpleDecals/Multiply" },
		};

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

		private void OnEnable()
		{
#if UNITY_EDITOR
			UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(projector, false);
#endif
		}

		public void Initialize(Transform hitObj, Vector3 position, Vector3 direction, DecalData decalData)
		{
			Axis axis = GetAxis(direction);
            transform.localPosition = GetPosition(hitObj, position, axis);
			transform.localEulerAngles = GetRotation(direction, axis);
            gameObject.SetActive(true);

			// TODO - Replace hack with PropetyBlock after Projector removal
			// TODO - Move to Shader.ToPropertyID
			Material mat = new Material(Shader.Find(s_ShaderFromBlendMode[decalData.blendMode]));
			mat.SetTexture("_DecalTex", decalData.texture);
			mat.SetInt("_Axis", (int)axis);
			projector.material = mat;
		}

		// -------------------------------------------------------------------
        // Position & Orientation

		private Axis GetAxis(Vector3 direction)
		{
			if(Mathf.Abs(direction.x) > 0.5)
			{
				return direction.x > 0 ? Axis.NegativeX : Axis.PositiveX;
			}
			else if(Mathf.Abs(direction.y) > 0.5)
			{
				return direction.y < 0 ? Axis.NegativeY : Axis.PositiveY;
			}
			else //if(Mathf.Abs(direction.z) > 0.5)
			{
				return direction.z > 0 ? Axis.NegativeZ : Axis.PositiveZ;
			}
		}

        private Vector3 GetPosition(Transform hitObj, Vector3 position, Axis axis)
		{
			Vector3 localPosition = hitObj.InverseTransformPoint(position);
			if(axis == Axis.PositiveX || axis == Axis.NegativeX)
			{
				return new Vector3 (0, localPosition.y, localPosition.z);
			}
			else if(axis == Axis.PositiveY || axis == Axis.NegativeY)
			{
				return new Vector3 (localPosition.x, 0, localPosition.z);
			}
			else //if(axis == Axis.PositiveZ || axis == Axis.NegativeZ)
			{
				return new Vector3(localPosition.x, localPosition.y, 0);
			}
		}

		private Vector3 GetRotation(Vector3 direction, Axis axis)
		{
			// TODO - Use this random rotation?
			float randomZ = 0;//UnityEngine.Random.Range(0, 360);
			switch(axis)
			{
				case Axis.NegativeX:
					return new Vector3(0, 90, randomZ);
				case Axis.PositiveX:
					return new Vector3(0, -90, randomZ);
				case Axis.NegativeY:
					return new Vector3(90, 0, randomZ);
				case Axis.PositiveY:
					return new Vector3(-90, 0, randomZ);
				case Axis.NegativeZ:
					return new Vector3(0, 0, randomZ);
				default: //Axis.PositiveZ
					return new Vector3(0, 180, randomZ);
			}
		}
	}
}
