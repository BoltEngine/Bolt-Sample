using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bolt.Samples.GettingStarted
{
    [BoltGlobalBehaviour(BoltNetworkModes.Client, "Tutorial1")]
    public class ClientCallbacks : Bolt.GlobalEventListener
    {
        public override void BoltShutdownBegin(AddCallback registerDoneCallback)
        {
            registerDoneCallback(() =>
            {
                Debug.Log("Shutdown Done");
                SceneManager.LoadScene(0);
            });
        }
    }
}