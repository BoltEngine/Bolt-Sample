using System;
using System.Security.Cryptography;
using Bolt;
using Bolt.Matchmaking;
using Bolt.Utils;
using UdpKit;
using UnityEngine;

public class StreamReliableData : Bolt.GlobalEventListener
{
	[Range(1, 2048)]
	public int size = 2048;

	private int targetBandwidth = 100;
	private float timestamp = 0;

	private UdpChannelName testChannel;

	private byte[] data;
	private string hash;
	private bool ready = false;
	private bool canSend = false;

	public override void BoltStartBegin()
	{
		testChannel = BoltNetwork.CreateStreamChannel("test", UdpChannelMode.Reliable, 1);
	}

	public override void BoltStartDone()
	{
		if (BoltNetwork.IsServer)
		{
			BoltMatchmaking.CreateSession(sessionID: Guid.NewGuid().ToString());
		}
	}

	public override void SceneLoadRemoteDone(BoltConnection connection, IProtocolToken token)
	{
		BoltLog.Info("SceneLoadRemoteDone");

		if (BoltNetwork.IsClient)
		{
			ready = true;
			canSend = true;
		}
	}

	public override void Connected(BoltConnection connection)
	{
		connection.SetStreamBandwidth(1024 * targetBandwidth);
	}

	public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
	{
		foreach (var session in sessionList)
		{
			UdpSession photonSession = session.Value as UdpSession;

			if (photonSession.Source == UdpSessionSource.Photon)
			{
				BoltMatchmaking.JoinSession(photonSession);
			}
		}
	}

	private void OnGUI()
	{
		if (ready && BoltNetwork.IsClient)
		{
			GUILayout.BeginVertical(GUILayout.Width(Screen.width), GUILayout.Height(Screen.height));
			{
				GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
				{
					size = (int) GUILayout.HorizontalSlider(size, 1, 2048);
					GUILayout.Label(size.ToString());
				}
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
				{
					var lastValue = targetBandwidth;

					targetBandwidth = (int) GUILayout.HorizontalSlider(targetBandwidth, 1, 1000);
					GUILayout.Label(targetBandwidth.ToString());

					if (lastValue != targetBandwidth)
					{
						BoltNetwork.Server.SetStreamBandwidth(1024 * targetBandwidth);
						lastValue = targetBandwidth;
					}
				}
				GUILayout.EndHorizontal();

				if (GUILayout.Button("Send data", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
				{
					if (canSend)
					{
						canSend = false;
						GenerateData();

						timestamp = Time.time;

						BoltLog.Info("Sending data with hash {0} at {1}", hash, timestamp);
						BoltNetwork.Server.StreamBytes(testChannel, data);
					}
					else
					{
						BoltLog.Info("Waiting data transfer...");
					}
				}
			}
			GUILayout.EndVertical();
		}
	}

	public override void StreamDataStarted(BoltConnection connection, UdpChannelName channel, ulong streamID)
	{
		BoltLog.Warn("Connection {0} is transfering data on channel {1} :: Transfer {2}...", connection, channel, streamID);

		timestamp = Time.time;
	}

	public override void StreamDataProgress(BoltConnection connection, UdpChannelName channel, ulong streamID, float progress)
	{
		BoltLog.Info("[{3}%] Connection {0} is transfering data on channel {1} :: Transfer ID {2}", connection, channel, streamID, (int) (progress * 100));
	}

	public override void StreamDataAborted(BoltConnection connection, UdpChannelName channel, ulong streamID)
	{
		BoltLog.Error("Stream {0} on channel {1} from connection {2} has been aborted.", streamID, channel, connection);
	}

	public override void StreamDataReceived(BoltConnection connection, UdpStreamData data)
	{
		var diff = Time.time - timestamp;
		timestamp = 0;

		string localHash = CreateHash(data.Data);
		BoltLog.Info("Received data from channel {0}: {1} bytes [{2}] [{3}] in {4}", data.Channel, data.Data.Length, localHash, connection, diff);

		var evt = DataStreamCheck.Create(connection, ReliabilityModes.ReliableOrdered);
		evt.hash = localHash;
		evt.Send();
	}

	public override void OnEvent(DataStreamCheck evnt)
	{
		if (evnt.hash.Equals(hash))
		{
			var diff = Time.time - timestamp;
			timestamp = 0;
			BoltLog.Info("Other end received data: it's EQUAL in {0}secs", diff);
		}
		else
		{
			BoltLog.Error("Other end received data: it's NOT EQUAL");
		}

		canSend = true;
	}

	#region Data Manager

	private void GenerateData()
	{
		data = CreateData();
		hash = CreateHash(data);
	}

	private byte[] CreateData()
	{
		var data = new byte[1024 * size];
		var rand = new System.Random();

		rand.NextBytes(data);

		return data;
	}

	private string CreateHash(byte[] data)
	{
		string hash;
		using(SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
		{
			hash = Convert.ToBase64String(sha1.ComputeHash(data));
		}

		return hash;
	}

	#endregion
}
