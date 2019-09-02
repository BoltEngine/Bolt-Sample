using System;
using UnityEngine;
using UnityEngine.UI;

namespace Bolt.Samples.Photon.Lobby
{
    public class LobbyUITopPanel : MonoBehaviour, ILobbyUI
    {
        [SerializeField] private GameObject ui;
        [SerializeField] private Text statusLabel;
        [SerializeField] private Text regionLabel;
        [SerializeField] private Text hostLabel;
        [SerializeField] private Button backButton;
        
        private bool _isInGame = false;
        private bool _isDisplayed = true;

        private void OnEnable()
        {
            HideBackButton();
        }

        private void Update()
        {
            if (_isInGame == false)
                return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleVisibility(!_isDisplayed);
            }

        }

        public void SetInGame(bool value)
        {
            _isInGame = value;
        }

        public void SetHeaderInfo(string status = null, string host = null, string region = null)
        {
            statusLabel.text = status ?? statusLabel.text;
            hostLabel.text = host ?? hostLabel.text;
            regionLabel.text = region ?? regionLabel.text;
        }
        
        public void SetupBackButton(string label, Action callback)
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

        public void HideBackButton()
        {
            backButton.onClick.RemoveAllListeners();
            backButton.gameObject.SetActive(false);
        }
        
        public void ToggleVisibility(bool visible)
        {
            _isDisplayed = visible;
            ui.SetActive(visible);
        }
    }
}