using UnityEngine;
using UnityEngine.UI;

namespace Bolt.Samples.MoveAndShoot
{
	public class MoveAndShoot_PlayerLifeBar : Bolt.EntityBehaviour<IMoveAndShootPlayer>
	{
		[SerializeField] private Image lifeBar;
		[SerializeField] private Image teamFlag;
		[SerializeField] private Color teamA;
		[SerializeField] private Color teamB;

		private Camera _main;

		private void Start()
		{
			_main = Camera.main;
		}

		public override void Attached()
		{
			state.AddCallback("Health", OnHealthChanged);
			state.AddCallback("Team", OnTeamChanged);
		}

		private void LateUpdate()
		{
			transform.LookAt(_main.transform);
		}

		private void OnTeamChanged()
		{
			if (teamFlag != null)
			{
				var color = state.Team == 1 ? teamA : teamB;
				teamFlag.GetComponent<Image>().color = color;
			}
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
