using UnityEngine;
using UnityEngine.Serialization;

namespace Bolt.Samples.NetworkPaintStreamSample.Core
{
	public class ModelRotateController : Bolt.EntityBehaviour<ICharacterPaintState>
	{
		[FormerlySerializedAs("_rotateSpeed")] [SerializeField]
		private float rotateSpeed = 10;

		private Quaternion _targetRotation = Quaternion.identity;
		private readonly float _ajustSpeed = 1.5f;

		public override void Attached()
		{
			state.AddCallback("Rotation", OnRotationChanged);

			Debug.Log("ModelRotateController attached");
		}

		void Update()
		{
			if (entity.IsControlled && Input.GetMouseButton(1))
			{
				RotateCharacter();
			}

			if (entity.IsControlled == false && _targetRotation != Quaternion.identity)
			{
				transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, Time.deltaTime * _ajustSpeed);
			}
		}

		private void RotateCharacter()
		{
			var x = (Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime);
			var y = -(Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime);

			transform.Rotate(x, y, 0, Space.World);
		}

		// State Callbacks
		public void OnRotationChanged()
		{
			_targetRotation = state.Rotation;
		}
	}
}
