using System.Collections;
using System.Collections.Generic;
using Bolt;
using Photon.Bolt;
using UdpKit;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FailedToJoin
{
	[BoltGlobalBehaviour(BoltNetworkModes.Client, "Sample1x1_Game")]
	public class ClientCallbacks : GlobalEventListener
	{
		public override void SceneLoadLocalDone(string scene, IProtocolToken token)
		{
			Camera.main.backgroundColor = Color.green;
		
			Debug.LogFormat("Scene Load Done at {0}", scene);
		}

		public override void BoltShutdownBegin(AddCallback registerDoneCallback, UdpConnectionDisconnectReason disconnectReason)
		{
			registerDoneCallback(() => {
				SceneManager.LoadScene(0);
			});
		}
	}
}
