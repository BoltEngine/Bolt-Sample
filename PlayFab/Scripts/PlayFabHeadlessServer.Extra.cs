using System.Net;

namespace Bolt.Samples.PlayFab
{
	/// <summary>
	/// Static Data
	/// </summary>
	public partial class PlayFabHeadlessServer
	{
		private const string BindingConfigKey = "bolt_server";

#pragma warning disable IDE0051 // Remove unused private members
		private const string MessageInvalidArguments = "Please verify your execution arguments, this aplication should run in Headless Mode (use args: -batchmode -nographics) and optionally use -map to inform a valid Scene name";
		private const string MessageInvalidBinding = "Invalid Binding information, please verify your configuration";
		private const string MessageExceptionServer = "Exception while starting Server";
		private const string MessageBoltShutdown = "Bolt is shutting down";
		private const string MessageMaintenance = "Maintenance Shutdown";
#pragma warning restore IDE0051 // Remove unused private members

		/// <summary>
		/// Internal and External binding information 
		/// </summary>
		private class BindingInfo
		{
			public IPEndPoint externalInfo;
			public int internalServerPort;
		}

		/// <summary>
		/// Game Server configuration
		/// </summary>
		private struct HeadlessServerConfig
		{
			public string Map;
		}
	}
}