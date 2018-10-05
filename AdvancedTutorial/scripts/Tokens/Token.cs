using UnityEngine;
using System.Collections;
using System;

namespace Bolt.AdvancedTutorial
{
    public class TestToken : Bolt.IProtocolToken
	{
		static int NumberCounter;
		public int Number = 0;

		public TestToken ()
		{
			Number = ++NumberCounter;
		}

		void Bolt.IProtocolToken.Read (UdpKit.UdpPacket packet)
		{
			Number = packet.ReadInt ();
		}

		void Bolt.IProtocolToken.Write (UdpKit.UdpPacket packet)
		{
			packet.WriteInt (Number);
		}

		public override string ToString ()
		{
			return string.Format ("[TestToken {0}]", Number);
		}
	}
}
