
using System;
using UdpKit;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoltInitPro : Bolt.GlobalEventListener
{
	class RoomProtocolToken : Bolt.IProtocolToken
	{
		public String ArbitraryData;
		public String password;

		public void Read(UdpPacket packet)
		{
			ArbitraryData = packet.ReadString();
			password = packet.ReadString();
		}

		public void Write(UdpPacket packet)
		{
			packet.WriteString(ArbitraryData);
			packet.WriteString(password);
		}
	}

	enum State
	{
		SelectMode,
		SelectMap,
		EnterServerIp,
		StartServer,
		StartClient,
		Started,
	}

	State state;

	string map;
	string serverAddress = "127.0.0.1";
	int serverPort = 25000;

	private void Awake()
	{
#if !BOLT_CLOUD
		BoltLauncher.SetUdpPlatform(new DotNetPlatform());
#endif
	}
	
	void OnGUI()
	{
		Rect tex = new Rect(10, 10, 140, 75);
		Rect area = new Rect(10, 90, Screen.width - 20, Screen.height - 100);

		GUI.Box(tex, Resources.Load("BoltLogo") as Texture2D);
		GUILayout.BeginArea(area);

		switch (state)
		{
			case State.SelectMode:
				State_SelectMode();
				break;
			case State.SelectMap:
				State_SelectMap();
				break;
			case State.EnterServerIp:
				State_EnterServerIp();
				break;
			case State.StartClient:
				State_StartClient();
				break;
			case State.StartServer:
				State_StartServer();
				break;
		}

		GUILayout.EndArea();
	}

	private void State_EnterServerIp()
	{
		GUILayout.BeginHorizontal();

		GUILayout.Label("Server IP: ");
		serverAddress = GUILayout.TextField(serverAddress);

		GUILayout.Label("Server Port: ");
		serverPort = int.Parse(GUILayout.TextField(serverPort.ToString()));

		if (GUILayout.Button("Connect"))
		{
			state = State.StartClient;
		}

		GUILayout.EndHorizontal();
	}

	void State_SelectMode()
	{
		if (ExpandButton("Server"))
		{
			state = State.SelectMap;
		}
		if (ExpandButton("Client"))
		{
			state = State.EnterServerIp;
		}
	}

	void State_SelectMap()
	{
		foreach (string value in BoltScenes.AllScenes)
		{
			if (SceneManager.GetActiveScene().name != value)
			{
				if (ExpandButton(value))
				{
					map = value;
					state = State.StartServer;
				}
			}
		}
	}

	void State_StartServer()
	{
		BoltConfig config = BoltRuntimeSettings.instance.GetConfigCopy();
		// config.EnableIPv6 = true;

		BoltLauncher.StartServer(new UdpEndPoint(UdpIPv4Address.Any, (ushort) serverPort), config);
		state = State.Started;
	}

	void State_StartClient()
	{
		BoltConfig config = BoltRuntimeSettings.instance.GetConfigCopy();
		// config.EnableIPv6 = true;

		BoltLauncher.StartClient(UdpEndPoint.Any, config);
		state = State.Started;
	}

	bool ExpandButton(string text)
	{
		return GUILayout.Button(text, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
	}
	public override void BoltStartDone()
	{
		if (BoltNetwork.IsClient)
		{
#if !BOLT_CLOUD
			UdpEndPoint endPoint = new UdpEndPoint(UdpIPv4Address.Parse(serverAddress), (ushort) serverPort);
			BoltNetwork.Connect(endPoint);
#endif
		}
		else
		{
			BoltNetwork.LoadScene(map);
		}
	}
}