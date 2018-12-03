using UnityEngine;
using System.Collections;

namespace Bolt.AdvancedTutorial
{
	public class GameSpawnTimer : Bolt.GlobalEventListener
	{
		BoltEntity me;
		IPlayerState meState;

		[SerializeField]
		TypogenicText timer;

		public override void ControlOfEntityGained (BoltEntity arg)
		{
			if (arg.GetComponent<PlayerController> ()) {
				me = arg;
				meState = me.GetState<IPlayerState> ();
			}
		}

		public override void ControlOfEntityLost (BoltEntity arg)
		{
			if (arg.GetComponent<PlayerController> ()) {
				me = null;
				meState = null;
			}
		}

		void Update ()
		{
			// lock in middle of screen
			transform.position = Vector3.zero;

			// update timer
			if (me && meState != null) {
				if (meState.Dead) {
					timer.Set (Mathf.Max (0, (meState.respawnFrame - BoltNetwork.Frame) / BoltNetwork.FramesPerSecond).ToString ());
				} else {
					timer.Set ("");
				}
			} else {
				timer.Set ("");
			}
		}
	}
}