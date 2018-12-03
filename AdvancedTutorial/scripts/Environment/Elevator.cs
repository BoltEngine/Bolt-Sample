using UnityEngine;
using System.Collections;

namespace Bolt.AdvancedTutorial {

	public class Elevator : Bolt.EntityBehaviour<IElevatorState> {

	  [SerializeField]
	  Vector3 start;

	  [SerializeField]
	  Vector3 end;

	  [SerializeField]
	  float time = 10f;

	  void FixedUpdate() {
	    float t = Mathf.Clamp01(Mathf.PingPong(BoltNetwork.ServerTime, time) / time);
	    transform.position = Vector3.Lerp(start, end, t);
	  }
	}

}
