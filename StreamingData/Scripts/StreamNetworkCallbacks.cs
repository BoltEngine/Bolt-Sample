using System.Collections;
using System.Collections.Generic;
using System.Text;
using UdpKit;
using UnityEngine;

namespace Bolt.Samples.StreamData
{
	[BoltGlobalBehaviour]
	public class StreamNetworkCallbacks : Bolt.GlobalEventListener
	{
		private UdpChannelName testChannel;
		private string ID;
		private byte[] data;
		private byte[] buffer;

		private float updateInterval = 0.05f;
		private double lastInterval;

		public override void Connected(BoltConnection connection)
		{
			connection.SetStreamBandwidth(1024 * 2);
		}

		public override void BoltStartBegin()
		{
			buffer = new byte[2048];

			BoltLog.Info("Bolt Starting...");

			testChannel = BoltNetwork.CreateStreamChannel("CustomMessage", UdpKit.UdpChannelMode.Reliable, 4);

			ID = Random.Range(0, 100).ToString();

			if (BoltNetwork.IsClient)
			{
				data = Encoding.UTF8.GetBytes(string.Format("Hello from {0}", ID));
			}
			else if (BoltNetwork.IsServer)
			{
				data = Encoding.UTF8.GetBytes("Hello from Host");
			}
		}

		void Update()
		{
			float timeNow = Time.realtimeSinceStartup;
			if (timeNow > lastInterval + updateInterval)
			{
				BoltLog.Info("Timed action!");
				SendData();
				lastInterval = timeNow;
			}
		}

		public void SendData()
		{
			if (BoltNetwork.IsRunning)
			{
				if (BoltNetwork.IsClient)
				{
					new System.Random().NextBytes(buffer);
					BoltNetwork.Server.StreamBytes(testChannel, buffer);
				}

				if (BoltNetwork.IsServer)
				{
					foreach (var conn in BoltNetwork.Clients)
					{
						new System.Random().NextBytes(buffer);
						conn.StreamBytes(testChannel, data);
					}
				}
			}
		}

		public override void StreamDataReceived(BoltConnection connection, UdpStreamData data)
		{
			BoltLog.Info("Data received!");
		}
	}
}