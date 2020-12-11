using System.Text;
using UnityEngine;
using System.Linq;
using Photon.Bolt;
using Photon.Bolt.Utils;

namespace Bolt.Samples.StreamData
{
	[BoltGlobalBehaviour("StreamDataGameScene")]
	public class StreamNetworkCallbacks : GlobalEventListener
	{
		private string ID;

		private readonly float updateInterval = 1.0f;
		private double lastInterval;

		public override void BoltStartBegin()
		{
			BoltLog.Info("Bolt Starting...");

			ID = UnityEngine.Random.Range(0, 100).ToString();

			if (BoltNetwork.IsClient)
			{
				BoltLog.Info("Started Client");
			}
			else if (BoltNetwork.IsServer)
			{
				BoltLog.Info("Started Server");
			}
		}

		void Update()
		{
			float timeNow = Time.realtimeSinceStartup;
			if (timeNow > lastInterval + updateInterval && BoltNetwork.IsRunning)
			{
				SendData();
				lastInterval = timeNow;
			}

			RecvData();
		}

		private void RecvData()
		{
			if (BoltNetwork.IsClient)
			{
				if (BoltNetwork.Server != null)
				{
					RecvData(BoltNetwork.Server);
				}
			}

			if (BoltNetwork.IsServer)
			{
				foreach (var conn in BoltNetwork.Clients)
				{
					RecvData(conn);
				}
			}
		}

		public void RecvData(BoltConnection connection)
		{
			byte[] input;
			connection.ReceiveData(out input);

			if (input != null)
			{
				var info = Encoding.UTF8.GetString(input);

				BoltLog.Info(string.Format(":: {0}", info));
			}
		}

		public void SendData()
		{
			BoltLog.Info("Send!");

			var now = Time.timeSinceLevelLoad;

			if (BoltNetwork.IsClient && BoltNetwork.Server != null)
			{
				BoltLog.Info("Send action! Client at {0}", now);

				var info = string.Format("Client {0} says: {1}", ID, now);
				var data = Encoding.UTF8.GetBytes(info);

				BoltNetwork.Server.SendData(data);
			}

			if (BoltNetwork.IsServer && BoltNetwork.Clients.Any())
			{
				BoltLog.Info("Send action! Server at {0}", now);

				var info = string.Format("Server says: {0}", now);
				var data = Encoding.UTF8.GetBytes(info);

				foreach (var conn in BoltNetwork.Clients)
				{
					conn.SendData(data);
				}
			}
		}
	}
}
