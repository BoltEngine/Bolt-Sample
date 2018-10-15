using UnityEngine;
using System.Collections;

namespace Bolt.Samples.ServerMonitor
{
    [BoltGlobalBehaviour(BoltNetworkModes.Server, "Level1")]
    public class ServerCallbacks : Bolt.GlobalEventListener
    {
        public override void SceneLoadLocalDone(string map)
        {
            // instantiate server monitor stuff
            GameObject.Instantiate(Resources.Load("ServerMonitor"));
        }
    }
}