namespace Bolt.Samples.Zeuz
{
	using System.Collections;
	using UnityEngine;
	using UdpKit;
	using Bolt;
	using Bolt.zeuz;
	using global::udpkit.platform.photon.realtime;
	using global::Photon.Realtime;

	public class ZeuzClientCallbacks : GlobalEventListener
	{
		//========== PRIVATE MEMBERS ===================================================================================

		[SerializeField]
		private byte              m_MaxPlayersInRoom = 1;
		[SerializeField]
		private int               m_ZeuzServerGroupID;
		[SerializeField]
		private int[]             m_ZeuzGameProfiles;
		
		[SerializeField, Header("Debug")]
		private bool              m_ConnectToCustomServer;
		[SerializeField]
		private string            m_ServerIP;
		[SerializeField]
		private int               m_ServerPort;

		private EGameState        m_GameState;
		private EMatchmakingState m_MatchmakingState;
		private ZeuzClient        m_Client = new ZeuzClient();
		private bool              m_ServerInfoReceived;
		private string            m_IPAddress;
		private int               m_Port;
	
		//========== PUBLIC METHODS ====================================================================================

		public override void BoltStartDone()
		{
			m_GameState = EGameState.Playing;

#if BOLT_CLOUD
			BoltLog.Error("Can't connect to {0}:{1}. Direct connection is not available in free version!", m_IPAddress, m_Port);
#else
			BoltNetwork.Connect(new UdpEndPoint(UdpIPv4Address.Parse(m_IPAddress), (ushort)m_Port));
#endif
		}
		
		//========== PRIVATE METHODS ===================================================================================
		
		private IEnumerator JoinGame_Coroutine(int profileID)
		{
			m_GameState        = EGameState.Joining;
			m_MatchmakingState = EMatchmakingState.Joining;

			// Connect to Photon Cloud for matchmaking

			m_Client.AppId = BoltRuntimeSettings.instance.photonAppId;
			m_Client.ConnectToRegionMaster("eu");
			
			BoltLog.Info("Connecting to " + m_Client.AppId);

			// Wait up to 5 seconds for connection

			float timeLimit = Time.realtimeSinceStartup + 5.0f;
			while(m_Client.IsConnectedAndReady == false && Time.realtimeSinceStartup < timeLimit)
			{
				yield return null;
			}

			if(m_Client.IsConnectedAndReady == false)
			{
				BoltLog.Error("Failed to connect to Photon Cloud for matchmaking!");

				DisconnectFromMatchmaking();
				m_GameState = EGameState.Menu;

				yield break;
			}
		
			// Register delegates to receive info about dedicated game server
			// OnServerInfoReceived will be invoked when room is full and game server successfully reserved
			// If there is an error (services exhaustion, wrong setup, ...), OnServerNotAvailable is invoked 

			m_Client.OnServerInfoReceived += OnServerInfoReceived;
			m_Client.OnServerNotAvailable += OnServerNotAvailable;

			m_ServerInfoReceived = false;

			// Get RoomOptions instance directly from ZeuzClient or fill mandatory room properties yourself
			// Following code is a demonstration of joining a room for players who selected same Game Profile within same Server Group
			// You will need to come up with your custom matchmaking algorithm which suits your game
			
			RoomOptions roomOptions;
			if(m_ConnectToCustomServer == true)
			{
				roomOptions = m_Client.GetRoomOptions(m_ZeuzServerGroupID, profileID, m_ServerIP, m_ServerPort);
			}
			else
			{
				roomOptions = m_Client.GetRoomOptions(m_ZeuzServerGroupID, profileID);
			}
			roomOptions.IsOpen     = true;
			roomOptions.IsVisible  = true;
			roomOptions.MaxPlayers = m_MaxPlayersInRoom;

			string roomName = string.Format("Room_{0}_{1}", m_ZeuzServerGroupID, profileID);
			
			BoltLog.Info("Connected! Joining room: {0}, max players: {1}, Server Group ID: {2}, Game Profile ID: {3}", roomName, m_MaxPlayersInRoom, m_ZeuzServerGroupID, profileID);

			EnterRoomParams enterParams = new EnterRoomParams()
			{
				RoomName = roomName,
				RoomOptions = roomOptions,
				Lobby = TypedLobby.Default
			};

			m_Client.OpJoinOrCreateRoom(enterParams);
			
			// Wait up to 5 seconds and check if we're in the room

			timeLimit = Time.realtimeSinceStartup + 5.0f;
			while(m_Client.CurrentRoom == null && Time.realtimeSinceStartup < timeLimit)
			{
				yield return null;
			}

			if(m_ServerInfoReceived == true)
			{
				// Server info received too fast, sometimes happen with local server and Max Players In Room set to 1
				yield break;
			}

			if(m_Client.CurrentRoom == null)
			{
				BoltLog.Error("Failed to join room: {0}", roomName);

				DisconnectFromMatchmaking();
				m_GameState = EGameState.Menu;

				yield break;
			}

			BoltLog.Info("Joined room {0}, waiting for players...", m_Client.CurrentRoom.Name);

			m_MatchmakingState = EMatchmakingState.Queued;
		}

		private void OnServerInfoReceived(string ipAddress, int port)
		{
			m_ServerInfoReceived = true;

			BoltLog.Info("Received game server info! IP: {0}, Port: {1}", ipAddress, port);

			m_IPAddress = ipAddress;
			m_Port      = port;

			DisconnectFromMatchmaking();

			BoltLauncher.StartClient();
		}

		private void OnServerNotAvailable()
		{
			BoltLog.Error("Game server is not available!");

			DisconnectFromMatchmaking();

			m_GameState = EGameState.Menu;
		}

		private void Awake()
		{
			DontDestroyOnLoad(this);
#if !BOLT_CLOUD
			m_GameState = EGameState.Menu;
#endif
		}

		private void OnApplicationQuit()
		{
			DisconnectFromMatchmaking();

			BoltLauncher.Shutdown();
		}

		private void Update()
		{
			m_Client.Service();
		}

		private void OnGUI()
		{
			GUILayout.BeginArea(new Rect(200, 200, Screen.width - 400, Screen.height - 400));

			if(m_GameState == EGameState.None)
			{
#if BOLT_CLOUD
				GUILayout.Button("This sample works only with Bolt Pro!", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
#endif
			}
			else if(m_GameState == EGameState.Menu)
			{
				if(m_ZeuzServerGroupID == 0)
				{
					GUILayout.Button("Please fill Server Group ID to prefab ZeuzGame", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
				}
				else if(m_ZeuzGameProfiles == null || m_ZeuzGameProfiles.Length == 0)
				{
					GUILayout.Button("Please fill Game Profile IDs to prefab ZeuzGame", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
				}
				else
				{
					foreach(int profileID in m_ZeuzGameProfiles)
					{
						if(GUILayout.Button("Play Game Profile " + profileID, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
						{
							StartCoroutine(JoinGame_Coroutine(profileID));
						}
					}
				}
			}
			else if(m_GameState == EGameState.Joining)
			{
				string matchmakingStatus = "Unknown";

				if(m_MatchmakingState == EMatchmakingState.Joining)
				{
					matchmakingStatus = "Joining matchmaking...";
				}
				else if(m_MatchmakingState == EMatchmakingState.Queued)
				{
					matchmakingStatus = "Waiting for players";

					if(m_Client.CurrentRoom != null)
					{
						matchmakingStatus += string.Format(" ({0}/{1})", m_Client.CurrentRoom.PlayerCount, m_MaxPlayersInRoom);
					}
				}

				GUILayout.Button(matchmakingStatus, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
			}

			GUILayout.EndArea();
		}
		
		private void DisconnectFromMatchmaking()
		{
			m_Client.OnServerInfoReceived -= OnServerInfoReceived;
			m_Client.OnServerNotAvailable -= OnServerNotAvailable;

			m_Client.Disconnect();
		}

		//========== PRIVATE STRUCTURES ================================================================================

		private enum EGameState
		{
			None    = 0,
			Menu    = 1,
			Joining = 2,
			Playing = 3
		}

		private enum EMatchmakingState
		{
			None    = 0,
			Joining = 1,
			Queued  = 2
		}
	}
}
