using System.Collections;
using System.Collections.Generic;
using Bolt;
using UdpKit;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bolt.Samples.GettingStarted
{
	[BoltGlobalBehaviour(BoltNetworkModes.Client, "Tutorial1")]
	public class ClientCallbacks : Bolt.GlobalEventListener
	{
		public override void BoltShutdownBegin(AddCallback registerDoneCallback, UdpConnectionDisconnectReason disconnectReason)
		{
			registerDoneCallback(() =>
			{
				Debug.LogFormat("Shutdown Done with Reason: {0}", disconnectReason);
				SceneManager.LoadScene(0);
			});
		}
	}
}