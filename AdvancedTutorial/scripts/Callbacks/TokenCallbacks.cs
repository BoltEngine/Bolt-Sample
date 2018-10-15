using UnityEngine;
using System.Collections;

namespace Bolt.Samples.AdvancedTutorial
{
    [BoltGlobalBehaviour]
    public class TokenCallbacks : Bolt.GlobalEventListener
    {
        public override void BoltStartBegin()
        {
            BoltNetwork.RegisterTokenClass<TestToken>();
        }
    }
}