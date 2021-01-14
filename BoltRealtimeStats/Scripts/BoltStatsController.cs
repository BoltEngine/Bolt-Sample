using System.Collections;
using System.Collections.Generic;
using Photon.Bolt;
using UnityEngine;
using UnityEngine.UI;

public class BoltStatsController : GlobalEventListener
{
	[SerializeField] private GameObject RemoteConnectionHolder;
	[SerializeField] private GameObject FrameStatPrefab;

	private Dictionary<BoltConnection, BoltFrameController> remoteFrameStateMap;

	void Start()
	{
		remoteFrameStateMap = new Dictionary<BoltConnection, BoltFrameController>();
	}

	void FixedUpdate()
	{
		if (BoltNetwork.IsRunning)
		{
			foreach (var conn in BoltNetwork.Connections)
			{
				if (remoteFrameStateMap.TryGetValue(conn, out var frameController) == false)
				{
					var frameControllerGO = Instantiate(FrameStatPrefab);
					frameControllerGO.transform.parent = RemoteConnectionHolder.transform;

					frameController = frameControllerGO.GetComponent<BoltFrameController>();
					frameController.SetLabel(conn);

					remoteFrameStateMap.Add(conn, frameController);
				}

				frameController.SetValue(conn.PingNetwork * 1000, conn.BitsPerSecondIn, conn.BitsPerSecondOut);
			}
		}
	}

	public override void Disconnected(BoltConnection connection)
	{
		if (remoteFrameStateMap.TryGetValue(connection, out var frameController))
		{
			Destroy(frameController.gameObject);
		}
	}
}
