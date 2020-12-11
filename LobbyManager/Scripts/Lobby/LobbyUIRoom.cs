using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Photon.Bolt;

namespace Bolt.Samples.Photon.Lobby
{
    //List of players in the lobby
    public class LobbyUIRoom : MonoBehaviour, ILobbyUI
    {
        [SerializeField] private RectTransform playerListContentTransform;
        [SerializeField] private Transform addButtonRow;

        private List<LobbyPlayer> _players = new List<LobbyPlayer>();

        public IEnumerable<LobbyPlayer> AllPlayers
        {
            get { return _players; }
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
            _players = new List<LobbyPlayer>();
        }

        public void ResetUI()
        {
            foreach (var player in _players)
            {
                if (player.entity.IsAttached && player.entity.IsOwner)
                {
                    BoltNetwork.Destroy(player.gameObject);
                }
            }
        }

        public void AddPlayer(LobbyPlayer player)
        {
            if (player == null) { return; }

            if (_players.Contains(player))
                return;

            _players.Add(player);
            player.transform.SetParent(playerListContentTransform, false);
            
            addButtonRow.transform.SetAsLastSibling();
            PlayerListModified();
        }

		public void RemovePlayer(LobbyPlayer player)
		{
			if (player == null) { return; }

            if (_players.Contains(player))
			{
                _players.Remove(player);
				PlayerListModified();
			}
        }

        public void PlayerListModified()
        {
            int i = 0;
            foreach (LobbyPlayer p in _players)
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
