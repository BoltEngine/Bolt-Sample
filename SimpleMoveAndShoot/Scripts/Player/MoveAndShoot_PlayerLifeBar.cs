using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bolt.Samples.MoveAndShoot
{
	public class MoveAndShoot_PlayerLifeBar : Bolt.EntityBehaviour<IMoveAndShootPlayer>
	{
		[SerializeField] private Image lifeBar;

		private Camera _main;

		private void Start()
		{
			_main = Camera.main;
		}

		public override void Attached()
		{
			state.AddCallback("Health", OnHealthChanged);
		}

		private void LateUpdate()
		{
			transform.LookAt(_main.transform);
		}

		private void OnHealthChanged()
		{
			if (lifeBar != null)
			{
				lifeBar.rectTransform.localScale = new Vector3(state.Health / 100f, 1, 1);
			}
		}
	}
}
