using System;
using System.Collections;
using UnityEngine;

namespace Bolt.AdvancedTutorial
{
	/// <summary>
	/// 
	/// </summary>
	public class TestToken : Bolt.IProtocolToken
	{
		static int NumberCounter;
		public int Number = 0;

		public TestToken()
		{
			Number = ++NumberCounter;
		}

		void Bolt.IProtocolToken.Read(UdpKit.UdpPacket packet)
		{
			Number = packet.ReadInt();
		}

		void Bolt.IProtocolToken.Write(UdpKit.UdpPacket packet)
		{
			packet.WriteInt(Number);
		}

		public override string ToString()
		{
			return string.Format("[TestToken {0}]", Number);
		}
	}
}