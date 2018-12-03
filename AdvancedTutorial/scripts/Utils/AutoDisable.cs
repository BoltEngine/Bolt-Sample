using UnityEngine;

namespace Bolt.AdvancedTutorial
{
	public class AutoDisable : MonoBehaviour
	{
		int remaining = 0;

		[SerializeField]
		int frames = 3;

		void OnEnable ()
		{
			remaining = frames;
		}

		void FixedUpdate ()
		{
			if (--remaining <= 0) {
				gameObject.SetActive (false);
			}
		}
	}
}
