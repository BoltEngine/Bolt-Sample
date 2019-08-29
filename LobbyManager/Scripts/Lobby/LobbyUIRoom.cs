using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Bolt.Samples.Photon.Lobby
{
    //List of players in the lobby
    public class LobbyUIRoom : MonoBehaviour, ILobbyUI
    {
        [SerializeField] private RectTransform playerListContentTransform;
        [SerializeField] private Transform addButtonRow;

        private VerticalLayoutGroup layout;
        private List<LobbyPlayer> players = new List<LobbyPlayer>();

//        public static bool Ready
//        {
//            get {  return _instance != null; }
//        }

        public IEnumerable<LobbyPlayer> AllPlayers
        {
            get { return players; }
        }

        public bool ServerIsPlaying
        {
            get { return ServerPlayer != null; }
        }

        public LobbyPlayer ServerPlayer
        {
            get;
            set;
        }

        public LobbyPlayer CreatePlayer()
        {
            if (!BoltNetwork.IsClient) { return null; }

            return null;
        }

        public void OnEnable()
        {
//            if (players != null)
//            {
//                foreach (var player in players)
//                {
//                    Destroy(player.gameObject);
//                }
//            }
            
//            _instance = this;
            players = new List<LobbyPlayer>();
            layout = playerListContentTransform.GetComponent<VerticalLayoutGroup>();
        }

//        public void DisplayDirectServerWarning(bool enabled)
//        {
//            if(warningDirectPlayServer != null)
//                warningDirectPlayServer.SetActive(enabled);
//        }

        void Update()
        {
            //this dirty the layout to force it to recompute evryframe (a sync problem between client/server
            //sometime to child being assigned before layout was enabled/init, leading to broken layouting)
            
//            if(layout)
//                layout.childAlignment = Time.frameCount%2 == 0 ? TextAnchor.UpperCenter : TextAnchor.UpperLeft;
        }

        public void ResetUI()
        {
            foreach (var player in players)
            {
                if (player.entity.IsAttached && player.entity.IsOwner)
                {
                    BoltNetwork.Destroy(player.gameObject);
                }
            }
        }

        public void AddPlayer(LobbyPlayer player)
        {
            if (players.Contains(player))
                return;

            players.Add(player);

//            player.OnDetach += RemovePlayer;
            player.transform.SetParent(playerListContentTransform, false);
            
            addButtonRow.transform.SetAsLastSibling();
            PlayerListModified();
        }

//        public void RemovePlayer(LobbyPlayer player)
//        {
//            if (gameObject.activeSelf)
//            {
//                StartCoroutine(LateDestroy(player));
//            }
//        }

//        private IEnumerator LateDestroy(LobbyPlayer player)
//        {
//            yield return new WaitUntil(() => player.entity.IsAttached == false);
            
//            players.Remove(player);
//            Destroy(player.gameObject);
//            PlayerListModified();
//        }

        public void PlayerListModified()
        {
            int i = 0;
            foreach (LobbyPlayer p in players)
            {
                p.OnPlayerListChanged(i);
                ++i;
            }
        }

        public void ToggleVisibility(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}
