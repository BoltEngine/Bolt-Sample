using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[BoltGlobalBehaviour(BoltNetworkModes.Server, "NetworkedPaint_Game")]
public class CharacterPaintServer : Bolt.GlobalEventListener
{
	void Start()
	{
	}

	void Update()
	{
	}

	public override void SceneLoadLocalDone(string scene, Bolt.IProtocolToken token)
	{
	}
}
