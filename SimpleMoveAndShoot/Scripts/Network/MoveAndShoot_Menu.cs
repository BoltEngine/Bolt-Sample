using System;
using Bolt.Matchmaking;
using Photon.Realtime;
using UdpKit;
using UnityEngine;
using UnityEngine.UI;

namespace Bolt.Samples.MoveAndShoot
{
	public class HitInfo : PooledProtocolToken
	{
		public Vector3 hitPosition;
		public bool hitType;

		public override void Read(UdpPacket packet)
		{
			hitPosition.x = packet.ReadFloat();
			hitPosition.y = packet.ReadFloat();
			hitPosition.z = packet.ReadFloat();
			hitType = packet.ReadBool();
		}

		public override void Write(UdpPacket packet)
		{
			packet.WriteFloat(hitPosition.x);
			packet.WriteFloat(hitPosition.y);
			packet.WriteFloat(hitPosition.z);
			packet.WriteBool(hitType);
		}

		public override void Reset()
		{
			hitPosition = Vector3.zero;
			hitType = false;
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
			BoltNetwork.RegisterTokenClass<HitInfo>();
		}

		public override void BoltStartDone()
		{
			if (BoltNetwork.IsServer)
			{
				var id = Guid.NewGuid().ToString().Split('-') [0];
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
