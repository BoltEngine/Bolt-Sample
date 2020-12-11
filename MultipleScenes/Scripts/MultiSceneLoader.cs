using System;
using System.Collections.Generic;
using Photon.Bolt;
using Photon.Bolt.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiSceneLoader : GlobalEventListener
{
	/// <summary>
	/// Stores the binding between the Action Button and the Scene to be loaded
	/// </summary>
	[Serializable]
	public struct LoadSceneBundle
	{
		public string SceneName;
		public Button ActionButton;
	}

	// List of scenes and Action Buttons
	[SerializeField] private LoadSceneBundle[] sceneBundles;

	// List of currently loaded scenes locally
	private List<string> loadedScenes;

	void Start()
	{
		loadedScenes = new List<string>();

		// On Server, we configure the buttons the load the Target Scenes
		if (BoltNetwork.IsServer)
		{
			foreach (var item in sceneBundles)
			{
				item.ActionButton.onClick.AddListener(() =>
				{
					if (loadedScenes.Contains(item.SceneName))
					{
						UnloadScene(item.SceneName);
					}
					else
					{
						LoadScene(item.SceneName);
					}
				});
			}
		}
		// On Client, we just disable the Buttons
		else if (BoltNetwork.IsClient)
		{
			foreach (var item in sceneBundles)
			{
				item.ActionButton.gameObject.SetActive(false);
			}
		}
	}

	/// <summary>
	/// On Destroy remove all button callbacks
	/// </summary>
	private void OnDestroy()
	{
		foreach (var item in sceneBundles)
		{
			item.ActionButton.onClick.RemoveAllListeners();
		}
	}

	/// <summary>
	/// Loads the Scene locally and request the clients to do the same
	/// </summary>
	/// <param name="sceneName">Target Scene Name</param>
	private void LoadScene(string sceneName)
	{
		if (BoltNetwork.IsServer)
		{
			var evt = LoadSceneRequest.Create(GlobalTargets.AllClients, ReliabilityModes.ReliableOrdered);
			evt.SceneName = sceneName;
			evt.Load = true;
			evt.Send();

			SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

			loadedScenes.Add(sceneName);
		}
	}

	/// <summary>
	/// Unloads the Scene locally and request the clients to do the same
	/// </summary>
	/// <param name="sceneName">Target Scene Name</param>
	private void UnloadScene(string sceneName)
	{
		if (BoltNetwork.IsServer)
		{
			var evt = LoadSceneRequest.Create(GlobalTargets.AllClients, ReliabilityModes.ReliableOrdered);
			evt.SceneName = sceneName;
			evt.Load = false;
			evt.Send();

			SceneManager.UnloadSceneAsync(sceneName);
			loadedScenes.Remove(sceneName);
		}
	}

	/// <summary>
	/// Runs only the client side.
	/// The Server requests that a certain scene to be loaded, and the client replies with Response
	/// confirming the scene load.
	/// </summary>
	public override void OnEvent(LoadSceneRequest evnt)
	{
		if (BoltNetwork.IsClient)
		{
			if (evnt.Load)
			{
				if (loadedScenes.Contains(evnt.SceneName) == false)
				{
					SceneManager.LoadSceneAsync(evnt.SceneName, LoadSceneMode.Additive);
					loadedScenes.Add(evnt.SceneName);
				}
			}
			else
			{
				SceneManager.UnloadSceneAsync(evnt.SceneName);
				loadedScenes.Remove(evnt.SceneName);
			}

			var evt = LoadSceneResponse.Create(GlobalTargets.OnlyServer);
			evt.SceneName = evnt.SceneName;
			evt.Load = evnt.Load;
			evt.Send();
		}
	}

	/// <summary>
	/// Runs only on the Server, just so signal that a remote client has loaded scene
	/// </summary>
	public override void OnEvent(LoadSceneResponse evnt)
	{
		if (BoltNetwork.IsServer)
		{
			if (evnt.Load)
			{
				BoltLog.Warn("Connection {0} has loaded scene {1}", evnt.RaisedBy, evnt.SceneName);
			}
			else
			{
				BoltLog.Warn("Connection {0} has unloaded scene {1}", evnt.RaisedBy, evnt.SceneName);
			}
		}
	}

	/// <summary>
	/// When a new client connects after the game was already started, this makes sure that it will
	/// load all already loaded additive scenes.
	/// </summary>
	public override void SceneLoadRemoteDone(BoltConnection connection, IProtocolToken token)
	{
		if (BoltNetwork.IsServer)
		{
			BoltLog.Warn("Remote Connection {0} has Loaded Scene", connection);

			foreach (var item in loadedScenes)
			{
				var evt = LoadSceneRequest.Create(connection, ReliabilityModes.ReliableOrdered);
				evt.SceneName = item;
				evt.Load = true;
				evt.Send();
			}
		}
	}
}
