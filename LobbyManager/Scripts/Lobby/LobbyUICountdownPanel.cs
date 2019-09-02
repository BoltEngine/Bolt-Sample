using UnityEngine;
using UnityEngine.UI;

namespace Bolt.Samples.Photon.Lobby
{
    public class LobbyUICountdownPanel : MonoBehaviour, ILobbyUI
    {
        [SerializeField] private Text uiText;

        public void SetText(string text)
        {
            uiText.text = text;
        }
        
        public void ToggleVisibility(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}