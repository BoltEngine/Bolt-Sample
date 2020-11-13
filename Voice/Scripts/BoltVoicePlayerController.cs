using System;
using UnityEngine;

namespace Bolt.Samples.Voice
{
	public class BoltVoicePlayerController : Bolt.EntityBehaviour<IVoicePlayer>
	{
		// ------------ UNITY MEMBERS ---------------------------------------------------------------------------------------

		[SerializeField] private float Speed = 4f;

		// ------------ PRIVATE MEMBERS -------------------------------------------------------------------------------------

#if BOLT_VOICE_SAMPLE
		private bool IsSpeakSetup = false;
		private const string VoiceAreaTag = "GameController";
#endif

		public override void Attached()
		{
			state.SetTransforms(state.Transform, transform);

			// Enable the AudioListener only for the local that has created the Entity
			GetComponent<AudioListener>().enabled = entity.IsOwner;
		}

		private void Update()
		{
			SetupSpeaker();
		}

		public override void SimulateOwner()
		{
			// Super simple movement, just translate the player

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

		/// <summary>
		/// Setup the Speaker per Player based on the Voice Player ID that is shared via a Bolt Property to all peers
		/// </summary>
		private void SetupSpeaker()
		{
#if BOLT_VOICE_SAMPLE
			// Ignore if already set
			if (IsSpeakSetup) { return; }

			// Setup the Voice Player ID on the Bolt Entity
			if (entity.IsOwner// Only Owner can change the state
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
#endif
		}

		#endregion

#if BOLT_VOICE_SAMPLE
		private void OnTriggerEnter(Collider other)
		{
			// If this player enters a Voice Area
			// gets the Area ID and change it using the BoltVoiceBridge
			if (entity.IsOwner)
			{
				if (BoltVoiceBridge.Instance == null) { return; }

				if (other.CompareTag(VoiceAreaTag))
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
			// Reset the Interest Group when the Player exits one Voice Area
			if (entity.IsOwner)
			{
				if (BoltVoiceBridge.Instance == null) { return; }

				if (other.CompareTag(VoiceAreaTag))
				{
					BoltVoiceBridge.Instance.ResetVoiceGroup();
				}
			}
		}
#endif
	}
}
