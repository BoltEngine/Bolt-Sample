using UnityEngine;
using System.Collections;
using System.Text;

namespace Bolt.AdvancedTutorial
{
	[BoltGlobalBehaviour("Level1")]
	public class PlayerCallbacks : Bolt.GlobalEventListener
	{
		public override void SceneLoadLocalDone(string scene)
		{
			// ui
			GameUI.Instantiate();

			// camera
			PlayerCamera.Instantiate();
		}

		public override void SceneLoadLocalBegin(string scene, Bolt.IProtocolToken token)
		{
			BoltLog.Info("SceneLoadLocalBegin-Token: {0}", token);
		}

		public override void SceneLoadLocalDone(string scene, Bolt.IProtocolToken token)
		{
			BoltLog.Info("SceneLoadLocalDone-Token: {0}", token);
		}

		public override void SceneLoadRemoteDone(BoltConnection connection, Bolt.IProtocolToken token)
		{
			BoltLog.Info("SceneLoadRemoteDone-Token: {0}", token);
		}

		public override void ControlOfEntityGained(BoltEntity entity)
		{
			// add audio listener to our character
			entity.gameObject.AddComponent<AudioListener>();

			// set camera callbacks
			PlayerCamera.instance.getAiming = () => entity.GetState<IPlayerState>().Aiming;
			PlayerCamera.instance.getHealth = () => entity.GetState<IPlayerState>().health;
			PlayerCamera.instance.getPitch = () => entity.GetState<IPlayerState>().pitch;

			// set camera target
			PlayerCamera.instance.SetTarget(entity);
		}
	}
}
