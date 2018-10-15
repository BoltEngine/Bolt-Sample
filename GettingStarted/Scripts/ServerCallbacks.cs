using UnityEngine;
using System.Collections;

namespace Bolt.Samples.GettingStarted
{
    [BoltGlobalBehaviour(BoltNetworkModes.Server)]
    public class GS_ServerCallbacks : Bolt.GlobalEventListener
    {
        public override void Connected(BoltConnection connection)
        {
            var log = LogEvent.Create();
            log.Message = string.Format("{0} connected", connection.RemoteEndPoint);
            log.Send();
        }

        public override void Disconnected(BoltConnection connection)
        {
            var log = LogEvent.Create();
            log.Message = string.Format("{0} disconnected", connection.RemoteEndPoint);
            log.Send();
        }
    }
}