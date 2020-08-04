using System;
using Bolt;
using Bolt.Matchmaking;
using Bolt.Photon;
using UdpKit;
using UnityEngine;

namespace FailedToJoin
{
	[BoltGlobalBehaviour(BoltNetworkModes.Server, "Sample1x1_Game")]
	public class ServerCallbacks : Bolt.GlobalEventListener
	{
		bool full;

		public override void SceneLoadLocalDone(string scene, IProtocolToken token)
		{
			Camera.main.backgroundColor = Color.clear;

			Debug.LogFormat("Scene Load Done at {0}", scene);
		}

		public override void ConnectRequest(UdpEndPoint endpoint, IProtocolToken token)
		{
			if (BoltNetwork.IsServer)
			{
				if (full == false)
				{
					full = true;

					PhotonRoomProperties roomProperties = new PhotonRoomProperties
					{
						IsOpen = false, 
						IsVisible = true
					};

					BoltMatchmaking.UpdateSession(roomProperties);

					BoltNetwork.Accept(endpoint);

					Debug.Log("Accept Client");
				}
				else
				{
					BoltNetwork.Refuse(endpoint);
					
					Debug.Log("Refuse Client");
				}
			}
		}

		public override void Disconnected(BoltConnection connection)
		{
			if (full == true)
			{
				full = false;

				PhotonRoomProperties roomProperties = new PhotonRoomProperties
				{
					IsOpen = true, IsVisible = true
				};

				BoltMatchmaking.UpdateSession(roomProperties);
			}
		}
	}
}