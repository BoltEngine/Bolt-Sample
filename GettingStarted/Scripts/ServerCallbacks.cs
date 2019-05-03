using System.Collections;
using System.Linq;
using Bolt.Tokens;
using UdpKit;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bolt.Samples.GettingStarted
{
	[BoltGlobalBehaviour(BoltNetworkModes.Server, "Tutorial1")]
	public class GS_ServerCallbacks : Bolt.GlobalEventListener
	{
		public override void Connected(BoltConnection connection)
		{
			var log = LogEvent.Create();
			log.Message = string.Format("{0} connected", connection.RemoteEndPoint);
			log.Send();
		}

		public override void Disconnected(BoltConnection connection)
		{
			var log = LogEvent.Create();
			log.Message = string.Format("{0} disconnected", connection.RemoteEndPoint);
			log.Send();
		}

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