﻿using UnityEngine;

namespace Bolt.Samples.Photon.Lobby
{
	[BoltGlobalBehaviour("PhotonGame")]
	public class PhotonGameSceneController : Bolt.GlobalEventListener
	{
		public override void SceneLoadLocalDone(string scene, IProtocolToken token)
		{
			BoltLog.Warn("Spawn Player on map {0}", scene);
			BomberPlayerController.Spawn();
		}
	}
}