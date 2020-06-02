using System;
using System.Collections;
using System.Collections.Generic;
using Bolt.Matchmaking;
using UnityEngine;
using UnityEngine.UI;

public class MultiSceneMenu : Bolt.GlobalEventListener
{
	// UI
	[SerializeField] private Button _startServerButton;
	[SerializeField] private Button _joinRandomButton;

	[SerializeField] private string _gameLevel;

	// Bolt
	private BoltConfig _config;

	void Awake()
	{
		Application.targetFrameRate = 60;
	}

	void Start()
	{
		_startServerButton.onClick.AddListener(StartServer);
		_joinRandomButton.onClick.AddListener(StartClient);

		_config = BoltRuntimeSettings.instance.GetConfigCopy();
		_config.disableAutoSceneLoading = true;
	}

	private void OnDestroy()
	{
		_startServerButton.onClick.RemoveAllListeners();
		_joinRandomButton.onClick.RemoveAllListeners();
	}

	void Update()
	{

	}

	private void StartServer()
	{
		BoltLauncher.StartServer(_config);
	}

	private void StartClient()
	{
		BoltLauncher.StartClient(_config);
	}

	// Bolt Events

	public override void BoltStartDone()
	{
		if (BoltNetwork.IsServer)
		{
			var id = Guid.NewGuid().ToString().Split('-')[0];
			var matchName = string.Format("{0} - {1}", id, _gameLevel);

			BoltMatchmaking.CreateSession(
				sessionID: matchName,
				sceneToLoad: _gameLevel
			);
		}
		else if (BoltNetwork.IsClient)
		{
			BoltMatchmaking.JoinRandomSession();
		}
	}

	public override void Connected(BoltConnection connection)
	{
		if (BoltNetwork.IsClient)
		{
			BoltNetwork.LoadSceneSync();
		}
	}
}
