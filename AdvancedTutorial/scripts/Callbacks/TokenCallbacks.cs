using UnityEngine;
using System.Collections;

namespace Bolt.AdvancedTutorial {

	[BoltGlobalBehaviour]
	public class TokenCallbacks : Bolt.GlobalEventListener {
	  public override void BoltStartBegin() {
	    BoltNetwork.RegisterTokenClass<TestToken>();
	  }
	}
}