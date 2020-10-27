using UdpKit;
using UnityEngine;

namespace Bolt.Samples.MoveAndShoot
{
	public class HitInfo : PooledProtocolToken
	{
		public Vector3 hitPosition;
		public bool hitType;

		public override void Read(UdpPacket packet)
		{
			hitPosition.x = packet.ReadFloat();
			hitPosition.y = packet.ReadFloat();
			hitPosition.z = packet.ReadFloat();
			hitType = packet.ReadBool();
		}

		public override void Write(UdpPacket packet)
		{
			packet.WriteFloat(hitPosition.x);
			packet.WriteFloat(hitPosition.y);
			packet.WriteFloat(hitPosition.z);
			packet.WriteBool(hitType);
		}

		public override void Reset()
		{
			hitPosition = Vector3.zero;
			hitType = false;
		}
	}
}
