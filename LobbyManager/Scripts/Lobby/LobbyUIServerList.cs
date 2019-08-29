using UnityEngine;
using System;
using UdpKit;

namespace Bolt.Samples.Photon.Lobby
{
    public class LobbyUIServerList : Bolt.GlobalEventListener, ILobbyUI
    {
        [SerializeField] private RectTransform serverListRect;
        [SerializeField] private GameObject serverEntryPrefab;
        [SerializeField] private GameObject noServerFound;

        public event Action<UdpSession> OnClickJoinSession;

        private int currentPage = 0;
        private int previousPage = 0;
        private int maxPageSize = 5;

        static readonly Color OddServerColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        static readonly Color EvenServerColor = new Color(.84f, .89f, .94f, 1.0f);

        private new void OnEnable()
        {
            base.OnEnable();

            currentPage = 0;
            previousPage = 0;

            ResetUI();
//			RequestPage(0);
        }

//		public void ChangePage(int dir)
//		{
//			int newPage = Mathf.Max(0, currentPage + dir);
//
//			//if we have no server currently displayed, need we need to refresh page0 first instead of trying to fetch any other page
//			if (noServerFound.activeSelf)
//				newPage = 0;
//
//			RequestPage(newPage);
//		}
//
//		public void RequestPage(int page)
//		{
//			previousPage = currentPage;
//			currentPage = page;
//		}

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