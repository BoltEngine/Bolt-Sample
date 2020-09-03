using System;
using Bolt.Matchmaking;
using UdpKit.Platform;
using UnityEngine;

namespace Bolt.Samples.NetworkPaintStreamSample.Network
{
	public class CharacterPaintMenu : Bolt.GlobalEventListener
	{
		private void Awake()
		{
			Application.targetFrameRate = 60;
			BoltLauncher.SetUdpPlatform(new PhotonPlatform());
		}

		public void StartServer()
		{
			BoltLauncher.StartServer();
		}

		public void StartClient()
		{
			BoltLauncher.StartClient();
		}

		public override void BoltStartDone()
		{
			if (BoltNetwork.IsServer)
			{
				string matchName = Guid.NewGuid().ToString();

				BoltMatchmaking.CreateSession(
					sessionID: matchName,
					sceneToLoad: "NetworkedPaint_Game"
				);
			}

			if (BoltNetwork.IsClient)
			{
				BoltMatchmaking.JoinRandomSession();
			}
		}
	}
}
