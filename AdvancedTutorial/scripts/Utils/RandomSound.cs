using UnityEngine;
using System.Collections;

namespace Bolt.AdvancedTutorial
{
	public enum RandomSoundMode
	{
		OnStart,
		OnFirstCollision
	}

	[RequireComponent (typeof(AudioSource))]
	public class RandomSound : MonoBehaviour
	{
		bool played = false;

		[SerializeField]
		AudioClip[] clips;

		[SerializeField]
		RandomSoundMode mode = RandomSoundMode.OnStart;

		void Start ()
		{
			if (mode == RandomSoundMode.OnStart) {
				Play ();
			}
		}

		void OnCollisionEnter (Collision c)
		{
			if (mode == RandomSoundMode.OnFirstCollision) {
				Play ();
			}
		}

		void Play ()
		{
			if (played) {
				return;
			}

			if (clips != null && clips.Length > 0) {
				GetComponent<AudioSource> ().PlayOneShot (clips [Random.Range (0, clips.Length)]);
			}

			played = true;
		}
	}
}
