using System;
using UnityEngine;
using UnityEngine.UI;

namespace Bolt.Samples.Photon.Lobby
{
    public class LobbyUIInfoPanel : MonoBehaviour, ILobbyUI
    {
        [SerializeField] private Text infoText;
        [SerializeField] private Text buttonText;
        [SerializeField] private Button singleButton;

        public void Display(string info, string buttonInfo = null, Action buttonCallback = null)
        {
            infoText.text = info;

            if (string.IsNullOrEmpty(buttonInfo) == false)
            {
                buttonText.text = buttonInfo;
                singleButton.gameObject.SetActive(true);
            }
            else
            {
                singleButton.gameObject.SetActive(false);
            }

            singleButton.onClick.RemoveAllListeners();

            if (buttonCallback != null)
            {
                singleButton.onClick.AddListener(buttonCallback.Invoke);
            }

            singleButton.onClick.AddListener(() => { ToggleVisibility(false); });

            ToggleVisibility(true);
        }

        public void ToggleVisibility(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}