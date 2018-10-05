using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Bolt.ServerMonitor
{
	// Simple class to represent server stats. Add any information that might be interesting to monitor,
	// then edit both ServerMonitor.cs and ClientMonitor.cs to set and show it respectively.

	// Google protobuf 2.x.x.x
	[ProtoContract]
	public class ServerStats
	{
		[ProtoMember(1)]
		public short Fps { get; set;}
		[ProtoMember(2)]
		public float Timestamp { get; set;}
		[ProtoMember(3)]
		public List<ClientStats> Clients { get; set; }

		public ServerStats () {
			Clients = new List<ClientStats> ();
		}

	}

	[ProtoContract]
	public class ClientStats {

		[ProtoMember(1)]
		public string IpAddress { get; set;}
		[ProtoMember(2)]
		public int Port { get; set;}
	}
}

