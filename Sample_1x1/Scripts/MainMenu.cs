using System;
using Photon.Bolt;
using Photon.Bolt.Matchmaking;
using Photon.Bolt.Utils;
using UdpKit;
using UdpKit.Platform.Photon;
using UnityEngine;

namespace FailedToJoin
{
	public class MainMenu : GlobalEventListener
	{
		Rect labelRoom = new Rect(0, 0, 140, 75);
		GUIStyle labelRoomStyle;

		void Awake()
		{
			Application.targetFrameRate = 60;

			labelRoomStyle = new GUIStyle()
			{
				fontSize = 20,
					fontStyle = FontStyle.Bold,
					normal = {
						textColor = Color.white
					}
			};
		}

		void Start()
		{
			// Get custom arguments from command line
			var IsClient = GetArg("-client");
			var IsServer = GetArg("-server");

			if (IsClient)
			{
				BoltLauncher.StartClient();
			}
			else if (IsServer)
			{
				BoltLauncher.StartServer();
			}
			else if (IsHeadlessMode())
			{
				Debug.LogError("Select one type of peer: -server or -client");
			}
		}

		public override void BoltStartBegin()
		{
			BoltNetwork.RegisterTokenClass<PhotonRoomProperties>();
		}

		public override void BoltStartDone()
		{
			if (BoltNetwork.IsServer)
			{
				PhotonRoomProperties token = new PhotonRoomProperties();
				token.IsOpen = true;
				token.IsVisible = true;

				var matchName = Guid.NewGuid().ToString();

				BoltMatchmaking.CreateSession(
					sessionID: matchName, 
					token: token, 
					sceneToLoad: "Game"
				);
			}
		}

		public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
		{
			BoltLog.Warn("{0} :: Session list updated: {1} total sessions", Time.frameCount, sessionList.Count);
		}

		public override void BoltShutdownBegin(AddCallback registerDoneCallback, UdpConnectionDisconnectReason disconnectReason)
		{
			BoltLog.Warn("Bolt Shutdown Begin with reason {0}", disconnectReason);
			Debug.LogFormat("Bolt Shutdown Begin with reason {0}", disconnectReason);
		}

		public override void SessionConnectFailed(UdpSession session, IProtocolToken token, UdpSessionError error)
		{
			BoltLog.Warn("Session Connect Failed");
			Debug.LogFormat("Session Connect Failed");
		}

		public override void ConnectFailed(UdpEndPoint endpoint, IProtocolToken token)
		{
			BoltLog.Warn("Connection Failed");
			Debug.LogFormat("Connection Failed");
		}

		public override void ConnectRefused(UdpEndPoint endpoint, IProtocolToken token)
		{
			BoltLog.Warn("Connection Refused");
			Debug.LogFormat("Connection Refused");
		}

		private void OnGUI()
		{
			if (IsHeadlessMode() == true) { return; }

			if (BoltNetwork.IsRunning == false)
			{
				GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, Screen.height - 20));

				if (GUILayout.Button("Server", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
				{
					BoltLauncher.StartServer();
				}

				if (GUILayout.Button("Client", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
				{
					BoltLauncher.StartClient();
				}

				GUILayout.EndArea();
			}

			if (BoltNetwork.IsClient)
			{
				State_SelectRoom();
			}
		}

		private System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
		private bool autoConnect = false;

		void State_SelectRoom()
		{
			GUI.Label(labelRoom, string.Format("Looking for rooms ({0}):", 5000 - watch.ElapsedMilliseconds), labelRoomStyle);

			if (BoltNetwork.SessionList.Count > 0)
			{
				if (watch.IsRunning == false)
				{
					BoltLog.Warn("Start connect timer");
					autoConnect = false;
					watch.Start();
				}

				if (watch.ElapsedMilliseconds > 5000)
				{
					autoConnect = true;
				}

				GUILayout.BeginVertical();
				GUILayout.Space(30);

				foreach (var session in BoltNetwork.SessionList)
				{
					var photonSession = session.Value as PhotonSession;

					if (photonSession.Source == UdpSessionSource.Photon)
					{
						var matchName = photonSession.HostName;
						var label = string.Format("Join: {0} | {1}/{2} | {3}", matchName, photonSession.ConnectionsCurrent, photonSession.ConnectionsMax, photonSession.IsOpen ? "Open" : "Closed");

						if ((ExpandButton(label) || autoConnect) && photonSession.IsOpen)
						{
							BoltMatchmaking.JoinSession(photonSession);

							autoConnect = false;
							watch.Stop();
							watch.Reset();
						}
					}
				}

				GUILayout.EndVertical();
			}
			else
			{
				watch.Stop();
				watch.Reset();
			}
		}

		bool ExpandButton(string text)
		{
			return GUILayout.Button(text, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
		}

		/// <summary>
		/// Utility function to detect if the game instance was started in headless mode.
		/// </summary>
		/// <returns><c>true</c>, if headless mode was ised, <c>false</c> otherwise.</returns>
		public static bool IsHeadlessMode()
		{
			return Environment.CommandLine.Contains("-batchmode") && Environment.CommandLine.Contains("-nographics");
		}

		static bool GetArg(params string[] names)
		{
			var args = Environment.GetCommandLineArgs();
			for (int i = 0; i < args.Length; i++)
			{
				foreach (var name in names)
				{
					if (args[i] == name)
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}
