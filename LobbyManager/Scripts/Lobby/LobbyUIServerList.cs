using UnityEngine;
using System;
using UdpKit;
using Photon.Bolt;

namespace Bolt.Samples.Photon.Lobby
{
    public class LobbyUIServerList : GlobalEventListener, ILobbyUI
    {
        [SerializeField] private RectTransform serverListRect;
        [SerializeField] private GameObject serverEntryPrefab;
        [SerializeField] private GameObject noServerFound;

        public event Action<UdpSession> OnClickJoinSession;

        static readonly Color OddServerColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        static readonly Color EvenServerColor = new Color(.84f, .89f, .94f, 1.0f);

        private new void OnEnable()
        {
            base.OnEnable();
            ResetUI();
        }

        public void ResetUI()
        {
            noServerFound.SetActive(true);
            
            foreach (Transform child in serverListRect)
            {
                Destroy(child.gameObject);
            }
        }

        public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
        {
            Debug.Log("Received session list update");

            ResetUI();
            
            if (sessionList.Count == 0)
            {
                noServerFound.SetActive(true);
                return;
            }

            noServerFound.SetActive(false);

            var i = 0;
            foreach (var pair in sessionList)
            {
                var session = pair.Value;
                var serverEntryGo = Instantiate(serverEntryPrefab, serverListRect, false);

                serverEntryGo.GetComponent<LobbyUIServerEntry>().Populate(session,
                    (i % 2 == 0) ? OddServerColor : EvenServerColor,
                    () =>
                    {
                        if (OnClickJoinSession != null) OnClickJoinSession.Invoke(session);
                    });

                ++i;
            }
        }

        public void ToggleVisibility(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}
