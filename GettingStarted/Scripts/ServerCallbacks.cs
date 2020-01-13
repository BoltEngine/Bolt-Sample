using UdpKit;
using UdpKit.Platform.Photon;
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
			log.Message = string.Format("{0} connected with token {1}", connection.RemoteEndPoint, connection.ConnectToken);
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

		public override void SessionCreated(UdpSession session)
		{
			Debug.LogWarning("Session created");
			
			var photonSession = session as PhotonSession;

			if (photonSession != null)
			{
				Debug.LogWarning(photonSession.HostName);
				Debug.LogWarning(photonSession.IsOpen);
				Debug.LogWarning(photonSession.IsVisible);

				foreach(var key in photonSession.Properties.Keys)
				{
					Debug.LogWarningFormat("{0} = {1}", key, photonSession.Properties[key]);
				}
			}
		}
	}
}