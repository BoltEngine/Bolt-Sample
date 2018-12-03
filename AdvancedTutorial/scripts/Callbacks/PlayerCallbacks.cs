using UnityEngine;
using System.Collections;
using System.Text;

namespace Bolt.AdvancedTutorial {

	[BoltGlobalBehaviour("Level1")]
	public class PlayerCallbacks : Bolt.GlobalEventListener {
	  public override void SceneLoadLocalDone(string map) {
	    // ui
	    GameUI.Instantiate();

	    // camera
	    PlayerCamera.Instantiate();
	  }

	  public override void SceneLoadLocalBegin(string scene, Bolt.IProtocolToken token) {
	    BoltLog.Info("SceneLoadLocalBegin-Token: {0}", token);
	  }

	  public override void SceneLoadLocalDone(string scene, Bolt.IProtocolToken token) {
	    BoltLog.Info("SceneLoadLocalDone-Token: {0}", token);
	  }

	  public override void SceneLoadRemoteDone(BoltConnection scene, Bolt.IProtocolToken token) {
	    BoltLog.Info("SceneLoadRemoteDone-Token: {0}", token);
	  }

	  public override void ControlOfEntityGained(BoltEntity arg) {
	    // add audio listener to our character
	    arg.gameObject.AddComponent<AudioListener>();

	    // set camera callbacks
	    PlayerCamera.instance.getAiming = () => arg.GetState<IPlayerState>().Aiming;
	    PlayerCamera.instance.getHealth = () => arg.GetState<IPlayerState>().health;
	    PlayerCamera.instance.getPitch = () => arg.GetState<IPlayerState>().pitch;

	    // set camera target
	    PlayerCamera.instance.SetTarget(arg);
	  }
	}

}
