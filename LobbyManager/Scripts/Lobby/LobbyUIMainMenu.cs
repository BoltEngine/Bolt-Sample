using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Bolt.Samples.Photon.Lobby
{
    public class LobbyUIMainMenu : MonoBehaviour, ILobbyUI
    {
//        [SerializeField] private LobbyManager lobbyManager;
//        [SerializeField] private RectTransform lobbyServerList;
//        [SerializeField] private RectTransform lobbyPanel;

        public event Action OnCreateButtonClick;
        public event Action OnBrowseServerClick;
        public event Action OnJoinRandomClick;

        public string MatchName
        {
            get { return matchNameInput.text; }
        }
        
        [Header("Server UI")]
        [SerializeField] private InputField matchNameInput;
        [SerializeField] private Button createRoomButton;

        [Header("Client UI")]
        [SerializeField] private Button browseServersButton;
        [SerializeField] private Button joinRandomButton;
        
        public void OnEnable()
        {
//            lobbyManager.topPanel.ToggleVisibility(true);
            
            createRoomButton.onClick.RemoveAllListeners();
            createRoomButton.onClick.AddListener(() =>
            {
                if (OnCreateButtonClick != null) OnCreateButtonClick();
            });
            
            browseServersButton.onClick.RemoveAllListeners();
            browseServersButton.onClick.AddListener(() =>
            {
                if (OnBrowseServerClick != null) OnBrowseServerClick();
            });
            
            joinRandomButton.onClick.RemoveAllListeners();
            joinRandomButton.onClick.AddListener(() =>
            {
                if (OnJoinRandomClick != null) OnJoinRandomClick();
            });

            matchNameInput.text = Guid.NewGuid().ToString();
        }

        public void ToggleVisibility(bool visible)
        {
            gameObject.SetActive(visible);
        }

        public void OnClickHost()
        {
            //lobbyManager.StartHost();
        }

        public void OnClickJoin()
        {
            //lobbyManager.ChangeTo(lobbyPanel);

            //lobbyManager.networkAddress = ipInput.text;
            //lobbyManager.StartClient();


            //lobbyManager.backDelegate = lobbyManager.StopClientClbk;
            //lobbyManager.DisplayIsConnecting();


            //lobbyManager.SetServerInfo("Connecting...", lobbyManager.networkAddress);
        }

        public void OnClickDedicated()
        {
            //lobbyManager.ChangeTo(null);
            //lobbyManager.StartServer();


            //lobbyManager.backDelegate = lobbyManager.StopServerClbk;

            //lobbyManager.SetServerInfo("Dedicated Server", lobbyManager.networkAddress);
        }

//        public void OnClickCreateMatchmakingGame()
//        {
//            lobbyManager.CreateMatch(matchNameInput.text);
//
//            lobbyManager.backDelegate = LobbyManager.Instance.Stop;
//            lobbyManager.DisplayIsConnecting();
//
//
//            lobbyManager.SetServerInfo("Matchmaker Host", LobbyManager.Instance.matchHost);
//        }
//
//        public void OnClickOpenServerList()
//        {
//            lobbyManager.StartClient();
//            lobbyManager.backDelegate = lobbyManager.SimpleBackClbk;
//            lobbyManager.ChangeTo(lobbyServerList);
//        }

        public void OnClickJoinRandom()
        {
            //lobbyManager.StartClient();
            //lobbyManager.backDelegate = lobbyManager.SimpleBackClbk;
        }
    }
}
