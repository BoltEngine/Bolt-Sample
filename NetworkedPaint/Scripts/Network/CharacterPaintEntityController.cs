
using Photon.Bolt;

namespace Bolt.Samples.NetworkPaintStreamSample.Network
{
	public class CharacterPaintEntityController : EntityEventListener<ICharacterPaintState>
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
}
