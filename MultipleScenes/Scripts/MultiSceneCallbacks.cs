using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

[BoltGlobalBehaviour(BoltNetworkModes.Server | BoltNetworkModes.Client, "InGame_Main")]
public class MultiSceneCallbacks : Bolt.GlobalEventListener
{
	public override void SceneLoadLocalDone(string scene, IProtocolToken token)
	{
		var entity = BoltNetwork.Instantiate(BoltPrefabs.MultiScenePlayer, Vector3.zero, Quaternion.identity);
	}
}
