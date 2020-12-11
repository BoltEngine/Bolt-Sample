using Photon.Bolt;
using Photon.Bolt.Matchmaking;
using UdpKit.Platform;
using UnityEngine;

namespace Bolt.Samples.PlayFab
{
	public class PlayFabMenuClient : GlobalEventListener
	{
		private bool _showGui = true;

		private void Awake()
		{
			Application.targetFrameRate = 60;
			BoltLauncher.SetUdpPlatform(new PhotonPlatform());
		}

		private void OnGUI()
		{
			if (!_showGui) { return; }

			GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, Screen.height - 20));

			if (GUILayout.Button("Start Client", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
			{
				// START CLIENT
				BoltLauncher.StartClient();
			}

			GUILayout.EndArea();
		}

		public override void BoltStartBegin()
		{
			_showGui = false;
			BoltNetwork.RegisterTokenClass<PhotonRoomProperties>();
		}

		public override void BoltStartDone()
		{
			if (BoltNetwork.IsClient)
			{
				BoltMatchmaking.JoinRandomSession();
			}
		}
	}
}
