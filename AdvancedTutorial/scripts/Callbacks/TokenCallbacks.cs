using UnityEngine;
using System.Collections;
using Photon.Bolt;

namespace Bolt.AdvancedTutorial {

	[BoltGlobalBehaviour]
	public class TokenCallbacks : GlobalEventListener {
	  public override void BoltStartBegin() {
	    BoltNetwork.RegisterTokenClass<TestToken>();
	  }
	}
}
