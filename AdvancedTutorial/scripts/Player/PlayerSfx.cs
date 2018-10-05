using UnityEngine;
using System.Collections;

namespace Bolt.AdvancedTutorial
{
	public class PlayerSfx : Bolt.EntityBehaviour<IPlayerState>
	{
		int prevHealth;
		float playTime;

		[SerializeField]
		AudioSource _sourceFootSteps;

		[SerializeField]
		AudioSource _sourceHit;

		[SerializeField]
		AudioClip[] footSteps;

		[SerializeField]
		AudioClip hit;

		//[SerializeField]
		//float footStepsMinSpacing = 0.2f;

		void Update ()
		{
			playTime += Time.deltaTime;

			if (GetComponentInChildren<Animator> ().GetFloat ("Feets") > 2f) {
				if (playTime > 0.2f) {
					_sourceFootSteps.PlayOneShot (footSteps [Random.Range (0, footSteps.Length)]);
					playTime = 0f;
				}
			}
		}

		public override void ControlGained ()
		{
			prevHealth = 100;

			// assign callback
			state.AddCallback ("health", HealthChanged);
		}

		public override void ControlLost ()
		{
			state.RemoveCallback ("health", HealthChanged);
		}

		void HealthChanged ()
		{
			if ((state.health < prevHealth) && hit) {
				_sourceHit.PlayOneShot (hit);
			}

			prevHealth = state.health;
		}
	}
}
