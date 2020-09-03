using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

[BoltGlobalBehaviour(BoltNetworkModes.Server, "NetworkedPaint_Game")]
public class CharacterPaintServer : Bolt.GlobalEventListener
{
	public override void SceneLoadLocalDone(string scene, Bolt.IProtocolToken token)
	{
		var entity = BoltNetwork.Instantiate(BoltPrefabs.CharacterPainter);
		entity.TakeControl();
	}
}
