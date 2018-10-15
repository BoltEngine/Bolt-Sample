using UnityEngine;
using System.Collections;

namespace Bolt.Samples.AdvancedTutorial
{
    public static class ExtensionMethods
    {
        public static Player GetPlayer(this BoltConnection connection)
        {
            if (connection == null)
            {
                return Player.serverPlayer;
            }

            return (Player)connection.UserData;
        }
    }
}
