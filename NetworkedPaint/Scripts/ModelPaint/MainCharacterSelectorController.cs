using UnityEngine;

namespace Bolt.Samples.NetworkPaintStreamSample.Core
{
	public class MainCharacterSelectorController : MonoBehaviour
	{
		public Transform otherPlayersHolder;
		private int _lastCount;

		private void Awake()
		{
			BrokerSystem.OnMainCharacterChanged += OnMainCharacterChanged;
			BrokerSystem.OnAddOtherCharacter += OnAddOtherCharacter;
		}

		void OnDisable()
		{
			BrokerSystem.OnMainCharacterChanged -= OnMainCharacterChanged;
			BrokerSystem.OnAddOtherCharacter -= OnAddOtherCharacter;
		}

		private void Update()
		{
			UpdateOtherPlayersPositions();
		}

		private void OnMainCharacterChanged(GameObject go)
		{
			go.transform.SetParent(this.transform);
			go.transform.localPosition = Vector3.zero;
			go.transform.localScale = Vector3.one;
		}

		private void OnAddOtherCharacter(GameObject go)
		{
			go.transform.SetParent(otherPlayersHolder);
			go.transform.localScale = Vector3.one * 0.3f;
		}

		#region Utils

		private void UpdateOtherPlayersPositions()
		{
			if (otherPlayersHolder == null)
				return;

			var totalChildren = otherPlayersHolder.childCount;

			if (totalChildren == _lastCount)
				return;

			_lastCount = totalChildren;

			const float spacing = 1.5f;
			float diff = totalChildren > 1 ? (spacing * totalChildren) / 4 : 0;

			for (int i = 0; i < totalChildren; i++)
			{
				var child = otherPlayersHolder.GetChild(i);
				child.localPosition = new Vector3(0, spacing * i - diff, 0);
			}
		}

		#endregion
	}
}
