using UnityEngine;
using System.Collections;
using System;

namespace Bolt.AdvancedTutorial
{
	public static class UnityUtils
	{
		static readonly WaitForFixedUpdate WaitForFixed = new WaitForFixedUpdate ();

		public static IEnumerator DisableThenEnable (GameObject go, int frameDisable, int frameEnable)
		{
			while (frameEnable > BoltNetwork.serverFrame) {
				if (BoltNetwork.serverFrame >= frameDisable) {
					go.SetActive (false);
				}

				yield return WaitForFixed;
			}

			go.SetActive (true);
		}

		public static IEnumerator InFrames (int frames, Action action)
		{
			frames += BoltNetwork.serverFrame;

			while (frames > BoltNetwork.serverFrame) {
				yield return WaitForFixed;
			}

			action ();
		}
	}
}
