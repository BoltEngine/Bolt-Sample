using System.Collections;
using System.Collections.Generic;
using Bolt;
using Bolt.photon;
using Bolt.Samples.Photon.Simple;
using UnityEngine;

/// <summary>
/// Photon server updater.
/// 
/// This is an example class that will update the session information with 
/// new properties or new tokens with different data.
/// 
/// Uncomment the class annotation to get it working on your Bolt Server
/// 
/// </summary>

//[BoltGlobalBehaviour(BoltNetworkModes.Server, "Level1")]
public class PhotonServerUpdater : Bolt.GlobalEventListener
{
    public void Start()
    {
        StartCoroutine(UpdateSessionInfo());
    }

    IEnumerator UpdateSessionInfo()
    {
        while (true)
        {
            IProtocolToken token = null;

            switch (Random.Range(1, 3))
            {
                case 1:
                    token = new RoomProtocolToken()
                    {
                        ArbitraryData = string.Format("My DATA :: {0}", Random.Range(1, 100)),
                        password = "mysuperpass123"
                    };

                    break;

                case 2:
                    token = new PhotonRoomProperties();

                    ((PhotonRoomProperties)token).AddRoomProperty("t", Random.Range(1, 100));
                    ((PhotonRoomProperties)token).AddRoomProperty("m", Random.Range(1, 100));

                    break;
            }

            if (token != null)
            {
                BoltNetwork.SetServerInfo(null, token);
            }

            yield return new WaitForSeconds(30);
        }
    }
}
