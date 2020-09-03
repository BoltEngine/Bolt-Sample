﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPaintEntityController : Bolt.EntityEventListener<ICharacterPaintState>
{
	public override void Attached()
	{
		BrokerSystem.PublishAddOtherCharacter(gameObject);
	}

	public override void SimulateOwner()
	{
		state.Rotation = this.transform.localRotation;
	}

	public override void ControlGained()
	{
		BrokerSystem.PublishNewMainCharacter(gameObject);
	}
}
