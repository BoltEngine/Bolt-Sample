using System;
using Bolt.Matchmaking;
using UdpKit;
using UnityEngine;
using UnityEngine.UI;

namespace Bolt.Samples.MoveAndShoot
{
	public class MyCustomToken : PooledProtocolToken
	{
		public String Name;

		public override void Read(UdpPacket packet)
		{
			Name = packet.ReadString();


		}

		public override void Write(UdpPacket packet)
		{
			packet.WriteString(Name);

			// packet.WriteByteArray();
		}

		public override void Reset()
		{
			Name = default(string);
		}
	}

	public class MoveAndShoot_Menu : Bolt.GlobalEventListener
	{
		// UI
		[SerializeField] private Button startServerButton;
		[SerializeField] private Button joinRandomButton;

		[SerializeField] private string gameLevel;

		void Awake()
		{
			Application.targetFrameRate = 60;
		}

		void Start()
		{
			startServerButton.onClick.AddListener(StartServer);
			joinRandomButton.onClick.AddListener(StartClient);
		}

		private void OnDestroy()
		{
			startServerButton.onClick.RemoveAllListeners();
			joinRandomButton.onClick.RemoveAllListeners();
		}

		private void StartServer()
		{
			BoltLauncher.StartServer();
		}

		private void StartClient()
		{
			BoltLauncher.StartClient();
		}

		// Bolt Events

		public override void BoltStartBegin()
		{
			BoltNetwork.RegisterTokenClass<MyCustomToken>();
		}

		public override void BoltStartDone()
		{
			if (BoltNetwork.IsServer)
			{
				var id = Guid.NewGuid().ToString().Split('-')[0];
				var matchName = string.Format("{0} - {1}", id, gameLevel);

				BoltMatchmaking.CreateSession(
					sessionID: matchName,
					sceneToLoad: gameLevel
				);
			}
			else if (BoltNetwork.IsClient)
			{
				BoltMatchmaking.JoinRandomSession();
			}
		}
	}
}
