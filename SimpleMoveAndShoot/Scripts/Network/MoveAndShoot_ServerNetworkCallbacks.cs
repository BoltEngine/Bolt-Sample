using UnityEngine;
using Random = UnityEngine.Random;

namespace Bolt.Samples.MoveAndShoot
{
	[BoltGlobalBehaviour(BoltNetworkModes.Server, "SimpleMoveAndShoot_Game")]
	public class MoveAndShoot_ServerNetworkCallbacks : Bolt.GlobalEventListener
	{
		private GameObject[] _spawnPoints;

		public override void SceneLoadLocalDone(string scene, IProtocolToken token)
		{

			var entity = BoltNetwork.Instantiate(BoltPrefabs.MoveAndShootPlayer, GetSpawnPosition(), Quaternion.identity);
			entity.TakeControl();
		}

		public override void SceneLoadRemoteDone(BoltConnection connection, IProtocolToken token)
		{
			var entity = BoltNetwork.Instantiate(BoltPrefabs.MoveAndShootPlayer, GetSpawnPosition(), Quaternion.identity);
			entity.AssignControl(connection);
		}

		#region Utils

		private Vector3 GetSpawnPosition()
		{
			if (_spawnPoints == null)
			{
				_spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
			}

			var position = Vector3.zero;

			if (_spawnPoints != null)
			{
				var pos = Random.Range(0, _spawnPoints.Length);
				position = _spawnPoints[pos].transform.position;
			}

			return position;
		}

		#endregion
	}
}
