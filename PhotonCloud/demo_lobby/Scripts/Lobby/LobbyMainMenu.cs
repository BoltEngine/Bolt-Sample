using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Bolt.Samples.Photon.Lobby
{
    //Main menu, mainly only a bunch of callback called by the UI (setup throught the Inspector)
    public class LobbyMainMenu : MonoBehaviour 
    {
        public LobbyManager lobbyManager;

        public RectTransform lobbyServerList;
        public RectTransform lobbyPanel;

        public InputField matchNameInput;

        public void OnEnable()
        {
            lobbyManager.topPanel.ToggleVisibility(true);

            matchNameInput.onEndEdit.RemoveAllListeners();
            matchNameInput.onEndEdit.AddListener(OnEndEditGameName);
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

        public void OnClickCreateMatchmakingGame()
        {
            lobbyManager.CreateMatch(matchNameInput.text);

            lobbyManager.backDelegate = LobbyManager.s_Singleton.Stop;
            lobbyManager.DisplayIsConnecting();


            lobbyManager.SetServerInfo("Matchmaker Host", LobbyManager.s_Singleton.matchHost);
        }

        public void OnClickOpenServerList()
        {
            lobbyManager.StartClient();
            lobbyManager.backDelegate = lobbyManager.SimpleBackClbk;
            lobbyManager.ChangeTo(lobbyServerList);
        }

        public void OnClickJoinRandom()
        {
            //lobbyManager.StartClient();
            //lobbyManager.backDelegate = lobbyManager.SimpleBackClbk;
        }

        void OnEndEditGameName(string text)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnClickCreateMatchmakingGame();
            }
        }

    }
}
