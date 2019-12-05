using System.Net;

namespace Bolt.Samples.PlayFab
{
    public partial class PlayFabHeadlessServer
    {
        private const string BindingConfigKey = "bolt_server";

        private const string MessageInvalidArguments = "Please verify your execution arguments, this aplication should run in Headless Mode (use args: -batchmode -nographics) and optionally use -map to inform a valid Scene name";
        private const string MessageInvalidBinding = "Invalid Binding information, please verify your configuration";
        private const string MessageExceptionServer = "Exception while starting Server";
        private const string MessageBoltShutdown = "Bolt is shutting down";
        private const string MessageMaintenance = "Maintenance Shutdown";

        private class BindingInfo
        {
            public IPEndPoint externalInfo;
            public int internalServerPort;
        }

        private struct HeadlessServerConfig
        {
            public string Map;
            public bool AutoRun;
        }
    }
}