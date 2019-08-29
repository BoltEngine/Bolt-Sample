using System;
using System.Collections;
using Bolt.Matchmaking;
using UdpKit;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;

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
        
        [SerializeField] private GameObject addPlayerButton;
        [SerializeField] private Button backButton;

        private ILobbyUI currentPanel;

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
//            ChangeBodyTo(null);
        }
        
        private void ResetUI()
        {
//            uiRoom.ResetUI();
            uiServerList.ResetUI();
            
            uiInfoPanel.ToggleVisibility(false);
            uiTopPanel.ToggleVisibility(true);
            uiTopPanel.SetHeaderInfo("Offline", "None");
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
            
        }

        private void JoinSessionEvent(UdpSession session)
        {
            uiInfoPanel.Display("Connecting to Session...");
            JoinEventHandler(session);
        }
        
        private void SessionCreatedUIHandler(UdpSession session)
        {
            uiInfoPanel.ToggleVisibility(false);
            uiTopPanel.SetHeaderInfo("Host", "self");
            
            ChangeBodyTo(uiRoom);
        }

        private void ClientStaredUIHandler()
        {
            uiInfoPanel.ToggleVisibility(false);
            uiTopPanel.SetHeaderInfo("Client", "none");
            
            ChangeBodyTo(uiServerList);
        }

        private void ClientConnectedUIHandler()
        {
            uiInfoPanel.ToggleVisibility(false);
            uiTopPanel.SetHeaderInfo("Client", BoltMatchmaking.CurrentSession.HostName);
            
            ChangeBodyTo(uiRoom);
        }

        private void EntityAttachedEventHandler(BoltEntity entity)
        {
            var lobbyPlayer = entity.gameObject.GetComponent<LobbyPlayer>();
            uiRoom.AddPlayer(lobbyPlayer);
        }

        private void ChangeBodyTo(ILobbyUI newPanel)
        {
            if (currentPanel != null)
            {
                currentPanel.ToggleVisibility(false);
            }

            if (newPanel != null)
            {
                newPanel.ToggleVisibility(true);
            }

            currentPanel = newPanel;

            if (uiMainMenu == currentPanel as LobbyUIMainMenu)
            {
                HideBackButton();
            }
            else
            {
                SetupBackButton("Shutdown", ShutdownEventHandler);
            }
        }

        private void SetupBackButton(string label, Action callback)
        {
            var labelUi = backButton.GetComponentInChildren<Text>();
            if (labelUi)
            {
                labelUi.text = label;
            }
            
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(callback.Invoke);
            backButton.gameObject.SetActive(true);
        }

        private void HideBackButton()
        {
            backButton.onClick.RemoveAllListeners();
            backButton.gameObject.SetActive(false);
        }
        
        // Bolt Events
        
        public override void SceneLoadLocalDone(string scene)
        {
            BoltLog.Info(string.Format("New scene: {0}", scene));

            try
            {
                if (lobbyScene.SimpleSceneName == scene)
                {
                    ChangeBodyTo(uiMainMenu);
                    
                    HideBackButton();
                    uiTopPanel.SetInGame(false);
                }
                else
                {
                    ChangeBodyTo(null);
                    
                    uiTopPanel.SetInGame(true);
                    uiTopPanel.ToggleVisibility(false);
                    
                    SetupBackButton("Menu", ShutdownEventHandler);
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