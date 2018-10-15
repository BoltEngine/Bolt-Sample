using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Bolt;
using UdpKit;
using System;
using UnityEngine.SceneManagement;
using Bolt.Samples.Photon.Simple;

namespace Bolt.Samples.Photon.Lobby
{
    public class LobbyManager : Bolt.GlobalEventListener
    {
        static public LobbyManager s_Singleton;

        [Header("Lobby Configuration")]
        public SceneField lobbyScene;
        public SceneField gameScene;
        public int minPlayers = 2;

        [Header("UI Lobby")]
        [Tooltip("Time in second between all players ready & match start")]
        public float prematchCountdown = 5.0f;

        [Space]
        [Header("UI Reference")]
        public LobbyTopPanel topPanel;

        public RectTransform mainMenuPanel;
        public RectTransform lobbyPanel;

        public LobbyInfoPanel infoPanel;
        public LobbyCountdownPanel countdownPanel;
        public GameObject addPlayerButton;

        protected RectTransform currentPanel;

        public Button backButton;

        public Text statusInfo;
        public Text hostInfo;

        protected bool _isCountdown = false;
        protected string _matchName;

        public string matchHost
        {
            get
            {
                if (BoltNetwork.isRunning)
                {
                    if (BoltNetwork.isServer)
                    {
                        return "<server>";
                    }

                    if (BoltNetwork.isClient)
                    {
                        return "<client>";
                    }
                }
                return "";
            }
        }

        void Start()
        {
            s_Singleton = this;
            currentPanel = mainMenuPanel;

            backButton.gameObject.SetActive(false);
            GetComponent<Canvas>().enabled = true;

            DontDestroyOnLoad(gameObject);

            SetServerInfo("Offline", "None");

            BoltLog.Info("Lobby Scene: " + lobbyScene.SimpleSceneName);
            BoltLog.Info("Game Scene: " + gameScene.SimpleSceneName);
        }

        void FixedUpdate()
        {
            if (BoltNetwork.isServer && !_isCountdown)
            {
                VerifyReady();
            }
        }

        public override void SceneLoadLocalDone(string map)
        {
            BoltConsole.Write("New scene: " + map, Color.yellow);

            try
            {
                if (lobbyScene.SimpleSceneName == map)
                {
                    ChangeTo(mainMenuPanel);
                    topPanel.isInGame = false;
                }
                else if (gameScene.SimpleSceneName == map)
                {
                    ChangeTo(null);

                    backDelegate = Stop;
                    topPanel.isInGame = true;
                    topPanel.ToggleVisibility(false);

                    // Spawn Player
                    // SpawnGamePlayer();
                }

            }
            catch (Exception e)
            {
                BoltConsole.Write(e.Message, Color.red);
                BoltConsole.Write(e.Source, Color.red);
                BoltConsole.Write(e.StackTrace, Color.red);
            }
        }

        public void ChangeTo(RectTransform newPanel)
        {
            if (currentPanel != null)
            {
                currentPanel.gameObject.SetActive(false);
            }

            if (newPanel != null)
            {
                newPanel.gameObject.SetActive(true);
            }

            currentPanel = newPanel;

            if (currentPanel != mainMenuPanel)
            {
                backButton.gameObject.SetActive(true);
            }
            else
            {
                backButton.gameObject.SetActive(false);
                SetServerInfo("Offline", "None");
            }
        }

        public void DisplayIsConnecting()
        {
            var _this = this;
            infoPanel.Display("Connecting...", "Cancel", () => { _this.backDelegate(); });
        }

        public void SetServerInfo(string status, string host)
        {
            statusInfo.text = status;
            hostInfo.text = host;
        }

        public delegate void BackButtonDelegate();
        public BackButtonDelegate backDelegate;
        public void GoBackButton()
        {
            backDelegate();
            topPanel.isInGame = false;
        }

        // ----------------- Server management

        private void StartServer()
        {
            BoltLauncher.StartServer();
        }

        public void StartClient()
        {
            BoltLauncher.StartClient();
        }

        public void Stop()
        {
            BoltLauncher.Shutdown();
        }

        public void CreateMatch(string matchName, bool dedicated = false)
        {
            StartServer();
            _matchName = matchName;
        }

        public void SimpleBackClbk()
        {
            ChangeTo(mainMenuPanel);
        }

        // ----------------- Server callbacks ------------------

        public override void BoltStartBegin()
        {
            BoltNetwork.RegisterTokenClass<RoomProtocolToken>();
            BoltNetwork.RegisterTokenClass<ServerAcceptToken>();
            BoltNetwork.RegisterTokenClass<ServerConnectToken>();
        }

        public override void BoltStartDone()
        {
            if (!BoltNetwork.isRunning) { return; }

            if (BoltNetwork.isServer)
            {
                RoomProtocolToken token = new RoomProtocolToken()
                {
                    ArbitraryData = "My DATA",
                };

                BoltLog.Info("Starting Server");
                // Start Photon Room
                BoltNetwork.SetServerInfo(_matchName, token);

                // Setup Host
                infoPanel.gameObject.SetActive(false);
                ChangeTo(lobbyPanel);

                backDelegate = Stop;
                SetServerInfo("Host", "");

                // Build Server Entity
                BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.PlayerInfo);
                entity.TakeControl();

            }
            else if (BoltNetwork.isClient)
            {
                backDelegate = Stop;
                SetServerInfo("Client", "");
            }
        }

        public override void BoltShutdownBegin(AddCallback registerDoneCallback)
        {
            _matchName = "";

            if (BoltNetwork.isServer)
            {
                BoltNetwork.LoadScene(lobbyScene.SimpleSceneName);
            }
            else if (BoltNetwork.isClient)
            {
                SceneManager.LoadScene(lobbyScene.SimpleSceneName);
            }

            registerDoneCallback(() =>
            {
                BoltLog.Info("Shutdown Done");
                ChangeTo(mainMenuPanel);
            });
        }

        // --- Countdown management

        void VerifyReady()
        {
            if (!LobbyPlayerList.Ready) { return; }

            bool allReady = true;
            int readyCount = 0;

            foreach (LobbyPhotonPlayer player in LobbyPlayerList._instance.AllPlayers)
            {
                allReady = allReady && player.IsReady;

                if (!allReady) { break; }

                readyCount++;
            }

            if (allReady && readyCount >= minPlayers)
            {
                _isCountdown = true;
                StartCoroutine(ServerCountdownCoroutine());
            }
        }

        public IEnumerator ServerCountdownCoroutine()
        {
            float remainingTime = prematchCountdown;
            int floorTime = Mathf.FloorToInt(remainingTime);

            LobbyCountdown countdown;

            while (remainingTime > 0)
            {
                yield return null;

                remainingTime -= Time.deltaTime;
                int newFloorTime = Mathf.FloorToInt(remainingTime);

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

        // ----------------- Client callbacks ------------------

        public override void OnEvent(LobbyCountdown evnt)
        {
            countdownPanel.UIText.text = "Match Starting in " + evnt.Time;
            countdownPanel.gameObject.SetActive(evnt.Time != 0);
        }

        public override void EntityReceived(BoltEntity entity)
        {
            BoltConsole.Write("EntityReceived");
        }

        public override void EntityAttached(BoltEntity entity)
        {
            BoltConsole.Write("EntityAttached");

            if (!entity.isControlled)
            {
                LobbyPhotonPlayer photonPlayer = entity.gameObject.GetComponent<LobbyPhotonPlayer>();

                if (photonPlayer != null)
                {
                    photonPlayer.SetupOtherPlayer();
                }
            }
        }

        public override void Connected(BoltConnection connection)
        {
            if (BoltNetwork.isClient)
            {
                BoltConsole.Write("Connected Client: " + connection, Color.blue);

                infoPanel.gameObject.SetActive(false);
                ChangeTo(lobbyPanel);
            }
            else if (BoltNetwork.isServer)
            {
                BoltConsole.Write("Connected Server: " + connection, Color.blue);

                BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.PlayerInfo);

                LobbyPhotonPlayer lobbyPlayer = entity.GetComponent<LobbyPhotonPlayer>();
                lobbyPlayer.connection = connection;

                connection.UserData = lobbyPlayer;
                connection.SetStreamBandwidth(1024 * 1024);

                entity.AssignControl(connection);
            }
        }

        public override void Disconnected(BoltConnection connection)
        {
            LobbyPhotonPlayer player = connection.GetLobbyPlayer();
            if (player != null)
            {
                BoltLog.Info("Disconnected");

                player.RemovePlayer();
            }
        }

        public override void SceneLoadLocalBegin(string map)
        {
            base.SceneLoadLocalBegin(map);
        }
    }
}
