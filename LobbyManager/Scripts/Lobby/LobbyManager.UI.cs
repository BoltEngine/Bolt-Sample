using System;
using Photon.Bolt;
using Photon.Bolt.Matchmaking;
using Photon.Bolt.Utils;
using UdpKit;
using UnityEngine;
using UnityEngine.UI;

namespace Bolt.Samples.Photon.Lobby
{
    public partial class LobbyManager
    {
        [Space]
        [Header("UI Reference", order = 2)]
        [SerializeField] private LobbyUITopPanel uiTopPanel;
        [SerializeField] private LobbyUIMainMenu uiMainMenu;
        [SerializeField] private LobbyUIRoom uiRoom;
        [SerializeField] private LobbyUIServerList uiServerList;
        
        [SerializeField] private LobbyUIInfoPanel uiInfoPanel;
        [SerializeField] private LobbyUICountdownPanel uiCountdownPanel;
        
        private bool sceneFlag = false;
        private ILobbyUI _currentPanel;

        private void Update()
        {
            if (BoltNetwork.IsRunning && BoltMatchmaking.CurrentMetadata.ContainsKey("region"))
            {
                var region = BoltMatchmaking.CurrentMetadata["region"];

                if (region != null)
                {
                    uiTopPanel.SetHeaderInfo(null, null, ((string)region).ToUpper());
                }
            }
        }

        private void StartUI()
        {
            ResetUI();
            
            // Setup Main Menu
            uiMainMenu.OnCreateButtonClick += StartServerEvent;
            uiMainMenu.OnBrowseServerClick += StartClientEvent;
            uiMainMenu.OnJoinRandomClick += StartClientRandomEvent;
            uiMainMenu.ToggleVisibility(true);
            
            // Setup Browse Session
            uiServerList.OnClickJoinSession += JoinSessionEvent;  
        }

        private void LoadingUI()
        {
            uiInfoPanel.Display("Please wait...");
        }
        
        private void ResetUI()
        {
            uiServerList.ResetUI();
            
            uiInfoPanel.ToggleVisibility(false);
            uiTopPanel.ToggleVisibility(true);
            uiTopPanel.SetHeaderInfo("Offline", "None", "None");
            ChangeBodyTo(uiMainMenu);
        }

        private void StartServerEvent()
        {
            uiInfoPanel.Display("Creating Room...");
            StartServerEventHandler(uiMainMenu.MatchName);
        }

        private void StartClientEvent()
        {
            uiInfoPanel.Display("Connecting to Cloud...");
            StartClientEventHandler();
        }

        private void StartClientRandomEvent()
        {
            uiInfoPanel.Display("Connecting to Cloud...");
            StartClientEventHandler(true);
        }

        private void JoinSessionEvent(UdpSession session)
        {
            uiInfoPanel.Display("Connecting to Session...");
            JoinEventHandler(session);
        }
        
        private void SessionCreatedUIHandler(UdpSession session)
        {
            uiInfoPanel.ToggleVisibility(false);
            
            object region;
            BoltMatchmaking.CurrentMetadata.TryGetValue("region", out region);
            
            uiTopPanel.SetHeaderInfo("Host", "self", ((string) region).ToUpper());
            
            ChangeBodyTo(uiRoom);
        }

        private void ClientStaredUIHandler()
        {
            uiInfoPanel.ToggleVisibility(false);
            ChangeBodyTo(uiServerList);
            
            uiTopPanel.SetHeaderInfo("Client", "None", "None");
        }

        private void ClientConnectedUIHandler()
        {
            uiInfoPanel.ToggleVisibility(false);
            
            object region;
            BoltMatchmaking.CurrentMetadata.TryGetValue("region", out region);
            
            uiTopPanel.SetHeaderInfo("Client", BoltMatchmaking.CurrentSession.HostName, ((string) region).ToUpper());
            
            ChangeBodyTo(uiRoom);
        }

        private void EntityAttachedEventHandler(BoltEntity entity)
        {
            var lobbyPlayer = entity.gameObject.GetComponent<LobbyPlayer>();
            uiRoom.AddPlayer(lobbyPlayer);
        }

		private void EntityDetachedEventHandler(BoltEntity entity)
		{
            var lobbyPlayer = entity.gameObject.GetComponent<LobbyPlayer>();
            uiRoom.RemovePlayer(lobbyPlayer);
        }

        private void ChangeBodyTo(ILobbyUI newPanel)
        {
            if (_currentPanel != null)
            {
                _currentPanel.ToggleVisibility(false);
            }

            if (newPanel != null)
            {
                newPanel.ToggleVisibility(true);
            }

            _currentPanel = newPanel;

            if (uiMainMenu == _currentPanel as LobbyUIMainMenu)
            {
                uiTopPanel.HideBackButton();
            }
            else
            {
                uiTopPanel.SetupBackButton("Shutdown", ShutdownEventHandler);
            }
        }

        // Bolt Events

        public override void SceneLoadLocalDone(string scene, IProtocolToken token)
        {
            if (scene.Equals("PhotonLobby") && sceneFlag == false) { return; }

            BoltLog.Info(string.Format("New scene: {0}", scene));

            try
            {
                if (lobbyScene.SimpleSceneName == scene)
                {
                    ChangeBodyTo(uiMainMenu);
                    
                    uiTopPanel.HideBackButton();
                    uiTopPanel.SetInGame(false);
                    sceneFlag = true;
                }
                else
                {
                    ChangeBodyTo(null);
                    
                    uiTopPanel.SetInGame(true);
                    uiTopPanel.ToggleVisibility(false);
                    uiTopPanel.SetupBackButton("Menu", ShutdownEventHandler);
                    sceneFlag = false;
                }

            } catch (Exception e)
            {
                BoltLog.Error(e);
            }
        }
        
        public override void OnEvent(LobbyCountdown evt)
        {
            uiCountdownPanel.SetText(string.Format("Match Starting in {0}", evt.Time));
            uiCountdownPanel.ToggleVisibility(evt.Time != 0);
        }
    }
}
