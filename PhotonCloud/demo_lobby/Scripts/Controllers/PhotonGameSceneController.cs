using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[BoltGlobalBehaviour("PhotonGame")]
public class PhotonGameSceneController : Bolt.GlobalEventListener {

	public override void SceneLoadLocalDone(string map)
	{
        BoltConsole.Write("Spawn Player on map " + map, Color.yellow);
        BomberPlayerController.Spawn();
	}
}
