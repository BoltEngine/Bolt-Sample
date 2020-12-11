using UnityEngine;
using System.Collections;
using Photon.Bolt;

namespace Bolt.AdvancedTutorial {

	public class Building : EntityBehaviour<IBuildingState>
	{
	    public override void Attached()
	    {
	        state.SetTransforms(state.Transform, transform);
	    }
	}

}
