using UnityEngine;
using System.Collections;

namespace Bolt.Samples.AdvancedTutorial
{
	public class Building : Bolt.EntityBehaviour<IBuildingState>
	{
	    public override void Attached()
	    {
	        state.SetTransforms(state.Transform, transform);
	    }
	}
}
