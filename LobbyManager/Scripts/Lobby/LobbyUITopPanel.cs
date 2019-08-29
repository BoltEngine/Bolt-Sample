using System;
using UnityEngine;
using UnityEngine.UI;

namespace Bolt.Samples.Photon.Lobby
{
    public class LobbyUITopPanel : MonoBehaviour, ILobbyUI
    {
        public event Action OnBackButtonClick;
        
        [SerializeField] private GameObject ui;
        [SerializeField] private Text statusLabel;
        [SerializeField] private Text hostLabel;
        [SerializeField] private Button backButton;
        
        private bool isInGame = false;
        private bool isDisplayed = true;

        private void OnEnable()
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(() =>
            {
                if (OnBackButtonClick != null) OnBackButtonClick();
            });
            
            backButton.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (isInGame == false)
                return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleVisibility(!isDisplayed);
            }

        }

        public void SetInGame(bool value)
        {
            isInGame = value;
        }

        public void SetHeaderInfo(string status = null, string host = null)
        {
            statusLabel.text = status ?? statusLabel.text;
            hostLabel.text = host ?? hostLabel.text;
        }
        
        public void ToggleVisibility(bool visible)
        {
            isDisplayed = visible;
            ui.SetActive(visible);
        }
    }
}