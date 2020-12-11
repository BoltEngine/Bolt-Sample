using UnityEngine;
using System;
using UdpKit;
using UnityEngine.SceneManagement;
using UdpKit.Platform.Photon;
using Photon.Bolt;
using Photon.Bolt.Matchmaking;
using Photon.Bolt.Utils;

namespace Bolt.Samples
{
	public class BoltInit : GlobalEventListener
	{
		private enum State
		{
			SelectMode,
			SelectMap,
			SelectRoom,
			StartServer,
			StartClient,
			Started,
		}

		private Rect labelRoom = new Rect(0, 0, 140, 75);
		private GUIStyle labelRoomStyle;

		private State currentState;
		private string map;

		void Awake()
		{
			Application.targetFrameRate = 60;

			labelRoomStyle = new GUIStyle()
			{
				fontSize = 20,
				fontStyle = FontStyle.Bold,
				normal =
				{
					textColor = Color.white
				}
			};
		}

		void OnGUI()
		{
			Rect tex = new Rect(10, 10, 140, 75);
			Rect area = new Rect(10, 90, Screen.width - 20, Screen.height - 100);

			GUI.Box(tex, Resources.Load("BoltLogo") as Texture2D);

			if (BoltNetwork.IsRunning)
			{
				tex = new Rect(160, 10, 140, 75);
				GUILayout.BeginArea(tex);
				if (GUILayout.Button("Shutdown", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
				{
					BoltNetwork.Shutdown();
				}
				GUILayout.EndArea();
			}

			GUILayout.BeginArea(area);

			switch (currentState)
			{
				case State.SelectMode: State_SelectMode(); break;
				case State.SelectMap: State_SelectMap(); break;
				case State.SelectRoom: State_SelectRoom(); break;
				case State.StartClient: State_StartClient(); break;
				case State.StartServer: State_StartServer(); break;
			}

			GUILayout.EndArea();
		}

		void State_SelectRoom()
		{
			GUI.Label(labelRoom, "Looking for rooms:", labelRoomStyle);

			if (BoltNetwork.SessionList.Count > 0)
			{
				GUILayout.BeginVertical();
				GUILayout.Space(30);

				foreach (var session in BoltNetwork.SessionList)
				{
					var photonSession = session.Value as PhotonSession;

					if (photonSession.Source == UdpSessionSource.Photon)
					{
						var matchName = photonSession.HostName;
						var label = string.Format("Join: {0} | {1}/{2}", matchName, photonSession.ConnectionsCurrent, photonSession.ConnectionsMax);

						if (ExpandButton(label))
						{
							BoltMatchmaking.JoinSession(photonSession);
							currentState = State.Started;
						}
					}
				}

				GUILayout.EndVertical();
			}
		}

		void State_SelectMode()
		{
			if (ExpandButton("Server"))
			{
				currentState = State.SelectMap;
			}
			if (ExpandButton("Client"))
			{
				currentState = State.StartClient;
			}
		}

		void State_SelectMap()
		{
			GUILayout.BeginVertical();

			foreach (string value in BoltScenes.AllScenes)
			{
				if (SceneManager.GetActiveScene().name != value)
				{
					if (ExpandButton(value))
					{
						map = value;
						currentState = State.StartServer;
					}
				}
			}

			GUILayout.EndVertical();
		}

		void State_StartServer()
		{
			BoltLauncher.StartServer();
			currentState = State.Started;
		}

		void State_StartClient()
		{
			BoltLauncher.StartClient();
			currentState = State.SelectRoom;
		}

		public override void BoltStartBegin()
		{
			// Register any Protocol Token that are you using
			BoltNetwork.RegisterTokenClass<PhotonRoomProperties>();
		}

		public override void BoltStartDone()
		{
			if (BoltNetwork.IsServer)
			{
				var id = Guid.NewGuid().ToString().Split('-')[0];
				var matchName = string.Format("{0} - {1}", id, map);

				BoltMatchmaking.CreateSession(
					sessionID: matchName,
					sceneToLoad: map
				);
			}
		}

		public override void BoltShutdownBegin(AddCallback registerDoneCallback, UdpConnectionDisconnectReason disconnectReason)
		{
			registerDoneCallback(() =>
			{
				currentState = State.SelectMode;
			});
		}

		bool ExpandButton(string text)
		{
			return GUILayout.Button(text, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
		}

		public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
		{
			BoltLog.Info("New session list");
		}
	}
}
