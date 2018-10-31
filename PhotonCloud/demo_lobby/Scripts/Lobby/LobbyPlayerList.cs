using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Bolt.Samples.Photon.Lobby
{
    //List of players in the lobby
    public class LobbyPlayerList : MonoBehaviour
    {
        public static LobbyPlayerList _instance = null;

        public RectTransform playerListContentTransform;
        public GameObject warningDirectPlayServer;
        public Transform addButtonRow;

        protected VerticalLayoutGroup _layout;
        protected List<LobbyPhotonPlayer> _players = new List<LobbyPhotonPlayer>();

        public static bool Ready
        {
            get {  return _instance != null; }
        }

        public IEnumerable<LobbyPhotonPlayer> AllPlayers
        {
            get { return _players; }
        }

        public bool ServerIsPlaying
        {
            get { return ServerPlayer != null; }
        }

        public LobbyPhotonPlayer ServerPlayer
        {
            get;
            set;
        }

        public LobbyPhotonPlayer CreatePlayer()
        {
            if (!BoltNetwork.IsClient) { return null; }

            return null;
        }

        public void OnEnable()
        {
            _instance = this;
            _players = new List<LobbyPhotonPlayer>();
            _layout = playerListContentTransform.GetComponent<VerticalLayoutGroup>();
        }

        public void DisplayDirectServerWarning(bool enabled)
        {
            if(warningDirectPlayServer != null)
                warningDirectPlayServer.SetActive(enabled);
        }

        void Update()
        {
            //this dirty the layout to force it to recompute evryframe (a sync problem between client/server
            //sometime to child being assigned before layout was enabled/init, leading to broken layouting)
            
            if(_layout)
                _layout.childAlignment = Time.frameCount%2 == 0 ? TextAnchor.UpperCenter : TextAnchor.UpperLeft;
        }

        public void AddPlayer(LobbyPhotonPlayer player)
        {
            if (_players.Contains(player))
                return;

            _players.Add(player);

            player.transform.SetParent(playerListContentTransform, false);
            addButtonRow.transform.SetAsLastSibling();

            PlayerListModified();
        }

        public void RemovePlayer(LobbyPhotonPlayer player)
        {
            _players.Remove(player);

            PlayerListModified();
        }

        public void PlayerListModified()
        {
            int i = 0;
            foreach (LobbyPhotonPlayer p in _players)
            {
                p.OnPlayerListChanged(i);
                ++i;
            }
        }
    }
}
