using System.Collections;
using Bolt.Samples.Photon.Lobby.Utilities;
using Bolt.Samples.Photon.Simple;
using Photon.Bolt;
using Photon.Bolt.Matchmaking;
using Photon.Bolt.Utils;
using UdpKit;
using UdpKit.Platform;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bolt.Samples.Photon.Lobby
{
	public partial class LobbyManager : GlobalEventListener
	{
		public static LobbyManager Instance;

		[Header("Lobby Configuration", order = 0)]
		[SerializeField] private SceneField lobbyScene;
		[SerializeField] private SceneField gameScene;

		[SerializeField] private int minPlayers = 2;

		[Tooltip("Time in second between all players ready & match start")]
		[SerializeField]
		private float prematchCountdown = 5.0f;

		private bool isCountdown = false;
		private string matchName;
		private bool randomJoin = false;

		private void Awake()
		{
			BoltLauncher.SetUdpPlatform(new PhotonPlatform());
		}

		public new void OnEnable()
		{
			base.OnEnable();

			if (Instance == null)
			{
				Instance = this;
			}
			else if (Instance != this)
			{
				Destroy(gameObject);
			}

			DontDestroyOnLoad(gameObject);
		}

		private void Start()
		{
			StartUI();
			StartGamePlay();
		}

		private void StartGamePlay()
		{
			Debug.Log(string.Format("Lobby Scene: {0}", lobbyScene.SimpleSceneName));
			Debug.Log(string.Format("Game Scene: {0}", gameScene.SimpleSceneName));
		}

		// Game Loop

		private void FixedUpdate()
		{
			if (BoltNetwork.IsServer && isCountdown == false)
			{
				VerifyReady();
			}
		}

		// Countdown

		private void VerifyReady()
		{
			var allReady = true;
			var readyCount = 0;

			foreach (var entity in BoltNetwork.Entities)
			{
				if (entity.StateIs<ILobbyPlayerInfoState>() == false) continue;

				var lobbyPlayer = entity.GetState<ILobbyPlayerInfoState>();

				allReady &= lobbyPlayer.Ready;

				if (allReady == false) break;
				readyCount++;
			}

			if (allReady && readyCount >= minPlayers)
			{
				isCountdown = true;
				StartCoroutine(ServerCountdownCoroutine());
			}
		}

		private IEnumerator ServerCountdownCoroutine()
		{
			var remainingTime = prematchCountdown;
			var floorTime = Mathf.FloorToInt(remainingTime);

			LobbyCountdown countdown;

			while (remainingTime > 0)
			{
				yield return null;

				remainingTime -= Time.deltaTime;
				var newFloorTime = Mathf.FloorToInt(remainingTime);

				if (newFloorTime != floorTime)
				{
					floorTime = newFloorTime;

					countdown = LobbyCountdown.Create(GlobalTargets.Everyone);
					countdown.Time = floorTime;
					countdown.Send();
				}
			}

			countdown = LobbyCountdown.Create(GlobalTargets.Everyone);
			countdown.Time = 0;
			countdown.Send();

			BoltNetwork.LoadScene(gameScene.SimpleSceneName);
		}

		// Bolt Callbacks

		//// API

		private void StartServerEventHandler(string matchName)
		{
			this.matchName = matchName;
			BoltLauncher.StartServer();
		}

		private void StartClientEventHandler(bool randomJoin = false)
		{
			this.randomJoin = randomJoin;
			BoltLauncher.StartClient();
		}

		private void JoinEventHandler(UdpSession session)
		{
			if (BoltNetwork.IsClient)
			{
				BoltMatchmaking.JoinSession(session);
			}
		}

		private void ShutdownEventHandler()
		{
			BoltLauncher.Shutdown();
		}

		//// Callbacks

		public override void BoltStartBegin()
		{
			BoltNetwork.RegisterTokenClass<RoomProtocolToken>();
			BoltNetwork.RegisterTokenClass<ServerAcceptToken>();
			BoltNetwork.RegisterTokenClass<ServerConnectToken>();
		}

		public override void BoltStartDone()
		{
			if (BoltNetwork.IsServer)
			{
				var token = new RoomProtocolToken()
				{
					ArbitraryData = "My DATA",
				};

				BoltLog.Info("Starting Server");

				// Start Photon Room
				BoltMatchmaking.CreateSession(
						sessionID: matchName,
						token: token
				);
			}
			else if (BoltNetwork.IsClient)
			{
				if (randomJoin)
				{
					BoltMatchmaking.JoinRandomSession();
				}
				else
				{
					ClientStaredUIHandler();
				}

				randomJoin = false;
			}
		}

		public override void SessionCreatedOrUpdated(UdpSession session)
		{
			SessionCreatedUIHandler(session);

			// Build Server Entity
			var entity = BoltNetwork.Instantiate(BoltPrefabs.PlayerInfo);
			entity.TakeControl();
		}

		public override void BoltShutdownBegin(AddCallback registerDoneCallback, UdpConnectionDisconnectReason disconnectReason)
		{
			LoadingUI();

			matchName = "";

			if (lobbyScene.IsLoaded == false)
			{
				if (BoltNetwork.IsServer)
				{
					BoltNetwork.LoadScene(lobbyScene.SimpleSceneName);
				}
				else if (BoltNetwork.IsClient)
				{
					SceneManager.LoadScene(lobbyScene.SimpleSceneName);
				}
			}

			registerDoneCallback(() =>
			{
				Debug.Log("Shutdown Done");
				ResetUI();
			});
		}

		public override void EntityAttached(BoltEntity entity)
		{
			EntityAttachedEventHandler(entity);

			var photonPlayer = entity.gameObject.GetComponent<LobbyPlayer>();
			if (photonPlayer)
			{
				if (entity.IsControlled)
				{
					photonPlayer.SetupPlayer();
				}
				else
				{
					photonPlayer.SetupOtherPlayer();
				}
			}
		}

		public override void EntityDetached(BoltEntity entity)
		{
			EntityDetachedEventHandler(entity);
		}

		public override void Connected(BoltConnection connection)
		{
			if (BoltNetwork.IsClient)
			{
				BoltLog.Info("Connected Client: {0}", connection);
				ClientConnectedUIHandler();
			}
			else if (BoltNetwork.IsServer)
			{
				BoltLog.Info("Connected Server: {0}", connection);

				var entity = BoltNetwork.Instantiate(BoltPrefabs.PlayerInfo);
				entity.AssignControl(connection);
			}
		}

		public override void Disconnected(BoltConnection connection)
		{
			foreach (var entity in BoltNetwork.Entities)
			{
				if (entity.StateIs<ILobbyPlayerInfoState>() == false || entity.IsController(connection) == false) continue;

				var player = entity.GetComponent<LobbyPlayer>();

				if (player)
				{
					player.RemovePlayer();
				}
			}
		}

		public override void ConnectFailed(UdpEndPoint endpoint, IProtocolToken token)
		{
		}
	}
}
