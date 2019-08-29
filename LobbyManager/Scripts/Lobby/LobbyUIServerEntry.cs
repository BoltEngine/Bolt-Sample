using System;
using UnityEngine;
using UnityEngine.UI;
using UdpKit;

namespace Bolt.Samples.Photon.Lobby
{
    public class LobbyUIServerEntry : MonoBehaviour
    {
        [SerializeField] private Text serverInfoText;
        [SerializeField] private Text slotInfo;
        [SerializeField] private Button joinButton;

        public void Populate(UdpSession match, Color backgroundColor, Action clickAction)
        {
            serverInfoText.text = match.HostName;
            slotInfo.text = string.Format("{0}/{1}", match.ConnectionsCurrent, match.ConnectionsMax);

            joinButton.onClick.RemoveAllListeners();
            joinButton.onClick.AddListener(clickAction.Invoke);

            gameObject.GetComponent<Image>().color = backgroundColor;
        }
    }
}