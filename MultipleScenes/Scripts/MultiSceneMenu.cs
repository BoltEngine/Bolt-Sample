using System;
using Photon.Bolt;
using Photon.Bolt.Matchmaking;
using UnityEngine;
using UnityEngine.UI;

public class MultiSceneMenu : GlobalEventListener
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

		// Configure Bolt to not sync scenes automatically
		_config = BoltRuntimeSettings.instance.GetConfigCopy();
		_config.disableAutoSceneLoading = true;
	}

	private void OnDestroy()
	{
		_startServerButton.onClick.RemoveAllListeners();
		_joinRandomButton.onClick.RemoveAllListeners();
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

	/// <summary>
	/// When a client first connects to the server, we want it to sync at least the
	/// InGame scene, for this, we call BoltNetwork.LoadSceneSync()
	/// </summary>
	public override void Connected(BoltConnection connection)
	{
		if (BoltNetwork.IsClient)
		{
			BoltNetwork.LoadSceneSync();
		}
	}
}
