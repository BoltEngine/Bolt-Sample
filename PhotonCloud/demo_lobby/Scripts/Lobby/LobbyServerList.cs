using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections;
using System.Collections.Generic;
using System;
using UdpKit;

namespace Photon.Lobby
{
	public class LobbyServerList : Bolt.GlobalEventListener
	{
		public LobbyManager lobbyManager;

		public RectTransform serverListRect;
		public GameObject serverEntryPrefab;
		public GameObject noServerFound;

		protected int currentPage = 0;
		protected int previousPage = 0;

		static Color OddServerColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		static Color EvenServerColor = new Color(.94f, .94f, .94f, 1.0f);

		new void OnEnable()
		{
			base.OnEnable();

			currentPage = 0;
			previousPage = 0;

			foreach (Transform t in serverListRect)
				Destroy(t.gameObject);

			noServerFound.SetActive(false);

			RequestPage(0);
		}

		public void ChangePage(int dir)
		{
			int newPage = Mathf.Max(0, currentPage + dir);

			//if we have no server currently displayed, need we need to refresh page0 first instead of trying to fetch any other page
			if (noServerFound.activeSelf)
				newPage = 0;

			RequestPage(newPage);
		}

		public void RequestPage(int page)
		{
			previousPage = currentPage;
			currentPage = page;
		}

		public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
		{
			if (sessionList.Count == 0)
			{
				noServerFound.SetActive(true);
				return;
			}

			noServerFound.SetActive(false);
			foreach (Transform t in serverListRect)
				Destroy(t.gameObject);

			int i = 0;
			foreach (var pair in sessionList)
			{
				UdpSession udpSession = pair.Value;

				GameObject o = Instantiate(serverEntryPrefab) as GameObject;

				o.GetComponent<LobbyServerEntry>().Populate(udpSession, lobbyManager, (i % 2 == 0) ? OddServerColor : EvenServerColor);
				o.transform.SetParent(serverListRect, false);

				++i;
			}
		}
	}
}