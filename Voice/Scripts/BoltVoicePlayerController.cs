using System;
using Photon.Voice.Unity;
using UnityEngine;

namespace Bolt.Samples.Voice
{
	public class BoltVoicePlayerController : Bolt.EntityBehaviour<IVoicePlayer>
	{
		[SerializeField] private float Speed = 4f;

		private bool IsSpeakSetup = false;

		public override void Attached()
		{
			state.SetTransforms(state.Transform, transform);
		}

		public override void ControlGained()
		{
			GetComponent<AudioListener>().enabled = true;
		}

		private void Update()
		{
			SetupSpeaker();
		}

		public override void SimulateOwner()
		{
			var movement = Vector3.zero;

			if (Input.GetKey(KeyCode.W)) { movement.z += 1; }
			if (Input.GetKey(KeyCode.S)) { movement.z -= 1; }
			if (Input.GetKey(KeyCode.A)) { movement.x -= 1; }
			if (Input.GetKey(KeyCode.D)) { movement.x += 1; }

			if (movement != Vector3.zero)
			{
				transform.position = transform.position + (movement.normalized * Speed * BoltNetwork.FrameDeltaTime);
			}
		}

		#region Utils

		private void SetupSpeaker()
		{
			if (IsSpeakSetup) { return; }

			// Setup the Voice Player ID on the Bolt Entity
			if (entity.IsOwner // Only Owner can change the state
				&& state.VoicePlayerID == 0 // 0 mean the entity don't have a Voice ID yet
				&& BoltVoiceBridge.Instance != null // if Bridge is Ready
				&& BoltVoiceBridge.Instance.LocalPlayerID != -1 // if the Local Voice Player is connected
			)
			{
				state.VoicePlayerID = BoltVoiceBridge.Instance.LocalPlayerID;
			}

			if (BoltVoiceBridge.Instance != null)
			{
				GameObject speaker;
				if (BoltVoiceBridge.Instance.GetSpeaker(state.VoicePlayerID, out speaker))
				{
					if (speaker.transform.parent == null)
					{
						speaker.transform.parent = transform;
						speaker.transform.localPosition = Vector3.zero;

						IsSpeakSetup = true;
					}
				}
			}
		}

		#endregion

		private void OnTriggerEnter(Collider other)
		{
			if (entity.IsControlled)
			{
				if (BoltVoiceBridge.Instance == null) { return; }

				if (other.CompareTag("Finish"))
				{
					var voiceArea = other.GetComponent<BoltVoiceArea>();

					if (voiceArea != null)
					{
						BoltVoiceBridge.Instance.ChangeVoiceGround(voiceArea.VoiceGroup);
					}
				}
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (entity.IsControlled)
			{
				if (BoltVoiceBridge.Instance == null) { return; }

				if (other.CompareTag("Finish"))
				{
					BoltVoiceBridge.Instance.ResetVoiceGroup();
				}
			}
		}
	}
}
