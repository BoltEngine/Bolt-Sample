using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bolt.Samples.ClickToMove
{
	[BoltGlobalBehaviour(BoltNetworkModes.Server, "ClickToMoveGameScene")]
	public class ClickToMoveNetworkCallbacks : Bolt.GlobalEventListener
	{
		public override void SceneLoadLocalDone(string scene)
		{
			var player = InstantiateEntity();
			player.TakeControl();
		}

		public override void SceneLoadRemoteDone(BoltConnection connection)
		{
			var player = InstantiateEntity();
			player.AssignControl(connection);
		}

		private BoltEntity InstantiateEntity()
		{
			GameObject[] respawnPoints = GameObject.FindGameObjectsWithTag("Respawn");

			var respawn = respawnPoints[Random.Range(0, respawnPoints.Length)];
			return BoltNetwork.Instantiate(BoltPrefabs.EthanClickToMove, respawn.transform.position, Quaternion.identity);
		}
	}
}