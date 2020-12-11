using Photon.Bolt;
using UnityEngine;

namespace Bolt.Samples.ClickToMove
{
	[BoltGlobalBehaviour(BoltNetworkModes.Server, "ClickToMoveGameScene")]
	public class ClickToMoveNetworkCallbacks : GlobalEventListener
	{
		public override void SceneLoadLocalDone(string scene, IProtocolToken token)
		{
			var player = InstantiateEntity();
			player.TakeControl();
		}

		public override void SceneLoadRemoteDone(BoltConnection connection, IProtocolToken token)
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
