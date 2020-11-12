using System;
using System.Collections;
using Bolt.Matchmaking;
using Bolt.Photon;
using UdpKit;
using UdpKit.Platform;
using UdpKit.Platform.Photon;
using UnityEngine;

namespace Bolt.Samples.Voice
{
	public class BoltVoiceMenu : Bolt.GlobalEventListener
	{
		private bool _showGui = true;
		private Coroutine _timerRoutine;

		private void Awake()
		{
			Application.targetFrameRate = 60;
			BoltLauncher.SetUdpPlatform(new PhotonPlatform());
		}

		private void OnGUI()
		{
			if (!_showGui) { return; }

			GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, Screen.height - 20));

			if (GUILayout.Button("Start Server", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
			{
				// START SERVER
				BoltLauncher.StartServer();
			}

			if (GUILayout.Button("Start Client", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
			{
				// START CLIENT
				BoltLauncher.StartClient();
			}

			GUILayout.EndArea();
		}

		public override void BoltStartBegin()
		{
			_showGui = false;
			BoltNetwork.RegisterTokenClass<PhotonRoomProperties>();
		}

		public override void BoltStartFailed(UdpConnectionDisconnectReason disconnectReason)
		{
			_showGui = true;
			Debug.LogError("BoltStartFailed");
		}

		public override void BoltStartDone()
		{
			if (BoltNetwork.IsServer)
			{
				string matchName = Guid.NewGuid().ToString();

				BoltMatchmaking.CreateSession(
					sessionID: matchName,
					sceneToLoad: "Voice_Meeting"
				);
			}

			if (BoltNetwork.IsClient)
			{
				// This will start a server after 10secs of wait
				// if no server was found
				_timerRoutine = StartCoroutine(ShutdownAndStartServer());
			}
		}

		public override void BoltShutdownBegin(AddCallback registerDoneCallback, UdpConnectionDisconnectReason disconnectReason)
		{
			registerDoneCallback(() =>
			{
				BoltLauncher.StartServer();
			});
		}

		public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
		{
			if (_timerRoutine != null)
			{
				StopCoroutine(_timerRoutine);
				_timerRoutine = null;
			}

			Debug.LogFormat("Session list updated: {0} total sessions", sessionList.Count);

			foreach (var session in sessionList)
			{
				PhotonSession photonSession = session.Value as PhotonSession;

				if (photonSession != null && photonSession.Source == UdpSessionSource.Photon)
				{
					object value;
					if (photonSession.Properties.TryGetValue("type", out value))
					{
						BoltLog.Info("Room with type: {0}", value);
					}

					if (photonSession.Properties.TryGetValue("map", out value))
					{
						BoltLog.Info("Room with map: {0}", value);
					}

					BoltMatchmaking.JoinSession(photonSession);
				}
			}
		}

		#region Utils

		private static IEnumerator ShutdownAndStartServer(int timeout = 10)
		{
			yield return new WaitForSeconds(timeout);

			BoltLog.Warn("No server found, restarting");
			BoltNetwork.ShutdownImmediate();
		}

		#endregion
	}
}
