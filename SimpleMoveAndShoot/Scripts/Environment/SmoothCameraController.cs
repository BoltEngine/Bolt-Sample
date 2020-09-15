using UnityEngine;

namespace Bolt.Samples.MoveAndShoot
{
	public class SmoothCameraController : MonoBehaviour
	{
		[SerializeField] private Vector3 offset;
		[SerializeField] float speed = 30f;

		private static SmoothCameraController _instance;
		private Transform _target;
		private Vector3 _targetPosition;

		private void Awake()
		{
			if (_instance != null)
			{
				Destroy(_instance.gameObject);
			}

			_instance = this;
		}

		private void LateUpdate()
		{
			if (_target == null) { return; }

			_targetPosition = _target.position + offset;

			transform.position = Vector3.MoveTowards(transform.position, _targetPosition, speed * Time.deltaTime);
			transform.LookAt(_target);
		}

		public static void ChangeTarget(Transform target)
		{
			_instance._target = target;
		}
	}
}
