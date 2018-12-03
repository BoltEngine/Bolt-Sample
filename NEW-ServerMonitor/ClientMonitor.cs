using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using Bolt.ServerMonitor;
using ProtoBuf;

public class ClientMonitor : MonoBehaviour {

	private UdpClient client;
	private float updateInterval = 1;
	private float timer = 0;
	private IPEndPoint server;
	private volatile ServerStats stats = new ServerStats();

	void Start () {
		client = new UdpClient ();
		// endpoint for server
		IPEndPoint server = new IPEndPoint(IPAddress.Parse("127.0.0.1"), ServerMonitor.SERVER_PORT);
		client.Connect(server);
	}

	void OnGUI() {
		GUI.Label (new Rect (10,10,300,50), "fps: " + stats.Fps);
		GUI.Label (new Rect (10,40,300,50), "Clients:");
		int y = 70;
		foreach (ClientStats c in stats.Clients) {
			GUI.Label (new Rect (20,y,300,50), c.IpAddress);
			y += 30;
		}
	}
	
	void Update () {
		if (timer >= updateInterval) {
			timer = 0;
			// send stats query
			byte[] query = System.Text.Encoding.UTF8.GetBytes("get");
			client.Send(query, query.Length);
			UnityEngine.Debug.Log ("Sent query...");
			client.BeginReceive(new AsyncCallback(receiveCallback), null);
		}
		timer += Time.deltaTime;
	}
		
	private void receiveCallback(IAsyncResult res)
	{
		byte[] receivedData = client.EndReceive(res, ref server);
		stats = Serializer.Deserialize<ServerStats> (new MemoryStream (receivedData));
	}

	void OnDestroy() {
		client.Close ();
	}
}
