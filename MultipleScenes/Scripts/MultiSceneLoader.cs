using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiSceneLoader : Bolt.GlobalEventListener
{
	[Serializable]
	public struct LoadSceneBundle
	{
		public string SceneName;
		public Button ActionButton;
	}

	[SerializeField] private LoadSceneBundle[] sceneBundles;

	private List<string> loadedScenes;

	void Start()
	{
		loadedScenes = new List<string>();

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
		else if (BoltNetwork.IsClient)
		{
			foreach (var item in sceneBundles)
			{
				item.ActionButton.gameObject.SetActive(false);
			}
		}
	}

	private void OnDestroy()
	{
		foreach (var item in sceneBundles)
		{
			item.ActionButton.onClick.RemoveAllListeners();
		}
	}

	private void LoadScene(string sceneName)
	{
		if (BoltNetwork.IsServer)
		{
			var evt = LoadSceneRequest.Create(Bolt.GlobalTargets.AllClients, Bolt.ReliabilityModes.ReliableOrdered);
			evt.SceneName = sceneName;
			evt.Load = true;
			evt.Send();

			SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

			loadedScenes.Add(sceneName);
		}
	}

	private void UnloadScene(string sceneName)
	{
		if (BoltNetwork.IsServer)
		{
			var evt = LoadSceneRequest.Create(Bolt.GlobalTargets.AllClients, Bolt.ReliabilityModes.ReliableOrdered);
			evt.SceneName = sceneName;
			evt.Load = false;
			evt.Send();

			SceneManager.UnloadSceneAsync(sceneName);
			loadedScenes.Remove(sceneName);
		}
	}

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

			var evt = LoadSceneResponse.Create(Bolt.GlobalTargets.OnlyServer);
			evt.SceneName = evnt.SceneName;
			evt.Load = evnt.Load;
			evt.Send();
		}
	}

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
