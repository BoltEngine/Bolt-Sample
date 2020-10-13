using UnityEngine;
using Random = UnityEngine.Random;

namespace Bolt.Samples.MoveAndShoot
{
	[BoltGlobalBehaviour(BoltNetworkModes.Server, "SimpleMoveAndShoot_Game")]
	public class MoveAndShoot_ServerNetworkCallbacks : Bolt.GlobalEventListener
	{
		public override void SceneLoadLocalDone(string scene, IProtocolToken token)
		{
			var entity = BoltNetwork.Instantiate(BoltPrefabs.MoveAndShootPlayer, SpawnPointsManager.GetSpawnPosition(),
				Quaternion.identity);

			entity.TakeControl();
		}

		public override void SceneLoadRemoteDone(BoltConnection connection, IProtocolToken token)
		{
			var entity = BoltNetwork.Instantiate(BoltPrefabs.MoveAndShootPlayer, SpawnPointsManager.GetSpawnPosition(),
				Quaternion.identity);

			entity.AssignControl(connection);
		}
	}
}
