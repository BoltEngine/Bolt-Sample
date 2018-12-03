using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Example code to log the connections stats filtered by type
/// For each Bolt Connection, you need to enable the metrics as shown
/// in the 'Connected' callback.
/// Later, you can access and use the stats stored into the Bolt Connection
/// CommandsStats
/// EventsStats
/// StatesStats 
/// </summary>
//[BoltGlobalBehaviour(BoltNetworkModes.Server)]
public class NetworkListenner : Bolt.GlobalEventListener
{
  void FixedUpdate()
  {
    //if (BoltNetwork.isServer)
    //{
    //    foreach(BoltConnection connection in BoltNetwork.connections)
    //    {
    //        Debug.Log("Commands Total IN: " + connection.CommandsStats.TotalIn);
    //        Debug.Log("Commands Total OUT: " + connection.CommandsStats.TotalOut);
    //        Debug.Log("Commands IN: " + connection.CommandsStats.In);
    //        Debug.Log("Commands OUT: " + connection.CommandsStats.Out);
    //    }
    //}
  }

  public override void Connected(BoltConnection connection)
  {
    //if (BoltNetwork.isServer)
    //{
    //    connection.EnableMetrics = true;
    //}
  }

}
