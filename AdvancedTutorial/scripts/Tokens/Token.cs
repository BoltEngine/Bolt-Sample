using System;
using System.Collections;
using Photon.Bolt;
using UnityEngine;

namespace Bolt.AdvancedTutorial
{
	/// <summary>
	/// 
	/// </summary>
	public class TestToken : IProtocolToken
	{
		static int NumberCounter;
		public int Number = 0;

		public TestToken()
		{
			Number = ++NumberCounter;
		}

		void IProtocolToken.Read(UdpKit.UdpPacket packet)
		{
			Number = packet.ReadInt();
		}

		void IProtocolToken.Write(UdpKit.UdpPacket packet)
		{
			packet.WriteInt(Number);
		}

		public override string ToString()
		{
			return string.Format("[TestToken {0}]", Number);
		}
	}
}
