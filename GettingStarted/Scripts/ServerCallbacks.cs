using UnityEngine;
using System.Collections;
using Bolt.Tokens;
using UdpKit;
using UnityEngine.SceneManagement;
using System.Linq;

namespace Bolt.Samples.GettingStarted
{
    [BoltGlobalBehaviour(BoltNetworkModes.Server, "Tutorial1")]
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