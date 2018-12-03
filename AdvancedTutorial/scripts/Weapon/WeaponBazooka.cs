using UnityEngine;
using System.Collections;

namespace Bolt.AdvancedTutorial
{
	public class WeaponBazooka : WeaponBase
	{

		[SerializeField]
		public GameObject shellFakePrefab;

		[SerializeField]
		public GameObject shellEntityPrefab;

		[SerializeField]
		public float shellForce = 100;

		public override void Fx (BoltEntity entity)
		{
			// calculate where we are aiming
			Vector3 aimPos;
			Quaternion aimRot;
			PlayerCamera.instance.CalculateCameraAimTransform (entity.transform, entity.GetState<IPlayerState> ().pitch, out aimPos, out aimRot);

			// extrapolate that 1024 units forward
			Vector3 aimPosDistant = aimPos + (aimRot * Vector3.forward * 1024f);

			// then get the direction from the muzzle to the distant aim point
			Vector3 fireDirection = (aimPosDistant - muzzleFlash.position).normalized;

			// create shell
			GameObject shellGo = GameObject.Instantiate (shellFakePrefab, muzzleFlash.position, Quaternion.LookRotation (fireDirection)) as GameObject;

			// launch it
			shellGo.GetComponent<Rigidbody> ().AddRelativeForce (new Vector3 (0, 0, shellForce), ForceMode.VelocityChange);
			shellGo.GetComponentInChildren<ParticleSystem> ().Emit (20);
		}
	}
}
