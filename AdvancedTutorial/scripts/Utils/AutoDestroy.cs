using UnityEngine;

namespace Bolt.AdvancedTutorial
{
	public class AutoDestroy : MonoBehaviour
	{
		float remaining = 0;

		[SerializeField]
		float time = 3;

		void Awake ()
		{
			remaining = time;
		}

		void Update ()
		{
			if ((remaining -= Time.deltaTime) <= 0) {
				Destroy (gameObject);
			}
		}
	}
}
