using UnityEngine;
using System.Collections;
using System;
using Photon.Bolt;

namespace Bolt.AdvancedTutorial
{
	public static class UnityUtils
	{
		static readonly WaitForFixedUpdate WaitForFixed = new WaitForFixedUpdate();

		public static IEnumerator DisableThenEnable(GameObject go, int frameDisable, int frameEnable)
		{
			while (frameEnable > BoltNetwork.ServerFrame)
			{
				if (BoltNetwork.ServerFrame >= frameDisable)
				{
					go.SetActive(false);
				}

				yield return WaitForFixed;
			}

			go.SetActive(true);
		}

		public static IEnumerator InFrames(int frames, Action action)
		{
			frames += BoltNetwork.ServerFrame;

			while (frames > BoltNetwork.ServerFrame)
			{
				yield return WaitForFixed;
			}

			action();
		}
	}
}
