using Photon.Bolt;
using Photon.Bolt.Utils;
using UdpKit;

namespace Bolt.Samples.Photon.Simple
{
	[BoltGlobalBehaviour(BoltNetworkModes.Server, "PhotonGame")]
	public class PhotonServerCallbacks : GlobalEventListener
	{
		public override void Connected(BoltConnection connection)
		{
			BoltLog.Warn("Connected");

			ServerAcceptToken acceptToken = connection.AcceptToken as ServerAcceptToken;

			if (acceptToken != null)
			{
				BoltLog.Info("AcceptToken: " + acceptToken.GetType());
				BoltLog.Info("AcceptToken: " + acceptToken.data);
			}
			else
			{
				BoltLog.Warn("AcceptToken is NULL");
			}

			ServerConnectToken connectToken = connection.ConnectToken as ServerConnectToken;

			if (connectToken != null)
			{
				BoltLog.Info("ConnectToken: " + connectToken.GetType());
				BoltLog.Info("ConnectToken: " + connectToken.data);
			}
			else
			{
				BoltLog.Warn("ConnectToken is NULL");
			}
		}

		public override void ConnectAttempt(UdpEndPoint endpoint, IProtocolToken token)
		{
			BoltLog.Warn("Connect Attempt");
			base.ConnectAttempt(endpoint, token);
		}

		public override void ConnectFailed(UdpEndPoint endpoint, IProtocolToken token)
		{
			BoltLog.Warn("Connect Failed");
			base.ConnectFailed(endpoint, token);
		}

		public override void ConnectRefused(UdpEndPoint endpoint, IProtocolToken token)
		{
			BoltLog.Warn("Connect Refused");
			base.ConnectRefused(endpoint, token);
		}

		public override void ConnectRequest(UdpEndPoint endpoint, IProtocolToken token)
		{
			BoltLog.Warn("Connect Request");

			//token should be ServerConnectToken
			if (token != null)
			{
				BoltLog.Warn(token.GetType().ToString());

				ServerConnectToken t = token as ServerConnectToken;
				BoltLog.Warn("Server Token: null? " + (t == null));
				BoltLog.Warn("Data: " + t.data);

			}
			else
			{
				BoltLog.Warn("Received token is null");
			}

			ServerAcceptToken acceptToken = new ServerAcceptToken
			{
				data = "Accepted"
			};

			BoltNetwork.Accept(endpoint, acceptToken);
		}

		public override void Disconnected(BoltConnection connection)
		{
			BoltLog.Warn("Disconnected");
			base.Disconnected(connection);
		}
	}
}
