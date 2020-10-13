using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bolt.Samples.MoveAndShoot
{
	public class MoveAndShoot_NetworkCallbacks : Bolt.GlobalEventListener
	{
		[SerializeField] private GameObject HitDamageEffectPrefab;
		[SerializeField] private GameObject HitHealEffectPrefab;

		public override void OnEvent(MoveAndShootHitEvent evnt)
		{
			var hitData = evnt.HitData as HitInfo;

			if (hitData != null)
			{
				if (hitData.hitType == true)
				{
					Instantiate(HitHealEffectPrefab, hitData.hitPosition, Quaternion.identity);
				}
				else
				{
					Instantiate(HitDamageEffectPrefab, hitData.hitPosition, Quaternion.identity);
				}
			}
		}
	}
}
