using UdpKit;
using System;

namespace Bolt.Samples.Photon.Simple
{
	public class RoomProtocolToken : Bolt.IProtocolToken
	{
		public String ArbitraryData;
		public String password;

		public void Read(UdpPacket packet)
		{
			ArbitraryData = packet.ReadString();
			password = packet.ReadString();
		}

		public void Write(UdpPacket packet)
		{
			packet.WriteString(ArbitraryData);
			packet.WriteString(password);
		}
	}

	public class ServerAcceptToken : Bolt.IProtocolToken
	{
		public String data;

		public void Read(UdpPacket packet)
		{
			data = packet.ReadString();
		}

		public void Write(UdpPacket packet)
		{
			packet.WriteString(data);
		}
	}

	public class ServerConnectToken : Bolt.IProtocolToken
	{
		public String data;

		public void Read(UdpPacket packet)
		{
			data = packet.ReadString();
		}

		public void Write(UdpPacket packet)
		{
			packet.WriteString(data);
		}
	}
}