using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace BattleRobo
{
    /// <summary>
    /// This class extends Photon's PhotonPlayer object by custom properties.
    /// Provides several methods for setting and getting variables out of them.
    /// </summary>
    public static class PlayerStats
    {
        //keys for saving and accessing values in custom properties Hashtable
        public const string health = "health";
        public const string shield = "shield";
        public const string kills = "kills";
        public const string rank = "rank";


        /// <summary>
        /// Returns the networked player nick name.
        /// </summary>
        public static string GetName(this PhotonView player)
        {
            return player.owner.NickName;
        }

        /// <summary>
        /// Returns the networked health value of the player out of properties.
        /// </summary>
        public static int GetHealth(this PhotonView player)
        {
            return System.Convert.ToInt32(player.owner.CustomProperties[health]);
        }

        /// <summary>
        /// Synchronizes the health value of the player for all players via properties.
        /// </summary>
        public static void SetHealth(this PhotonView player, int value)
        {
            player.owner.SetCustomProperties(new Hashtable {{health, (byte) value}});
        }

        /// <summary>
        /// Returns the networked shield value of the player out of properties.
        /// </summary>
        public static int GetShield(this PhotonView player)
        {
            return System.Convert.ToInt32(player.owner.CustomProperties[shield]);
        }

        /// <summary>
        /// Synchronizes the shield value of the player for all players via properties.
        /// </summary>
        public static void SetShield(this PhotonView player, int value)
        {
            player.owner.SetCustomProperties(new Hashtable {{shield, (byte) value}});
        }

        /// <summary>
        /// Returns the networked kills value of the player out of properties.
        /// </summary>
        public static int GetKills(this PhotonView player)
        {
            return System.Convert.ToInt32(player.owner.CustomProperties[kills]);
        }

        /// <summary>
        /// Add one to the kills value of the player for all players via properties.
        /// </summary>
        public static void AddKills(this PhotonView player)
        {
            int pkills = player.GetKills();
            pkills++;

            player.SetKills(pkills);
        }

        /// <summary>
        /// Synchronizes the kills value of the player for all players via properties.
        /// </summary>
        public static void SetKills(this PhotonView player, int value)
        {
            player.owner.SetCustomProperties(new Hashtable {{kills, (byte) value}});
        }
        
        /// <summary>
        /// Returns the networked shield value of the player out of properties.
        /// </summary>
        public static int GetRank(this PhotonView player)
        {
            return System.Convert.ToInt32(player.owner.CustomProperties[rank]);
        }

        /// <summary>
        /// Synchronizes the shield value of the player for all players via properties.
        /// </summary>
        public static void SetRank(this PhotonView player, int value)
        {
            player.owner.SetCustomProperties(new Hashtable {{rank, (byte) value}});
        }

        /// <summary>
        /// Clears all networked variables of the player via properties in one instruction.
        /// </summary>
        public static void Clear(this PhotonView player)
        {
            player.owner.SetCustomProperties(new Hashtable
            {
                {health, (byte) 0},
                {kills, (byte) 0},
                {shield, (byte) 0}
            });
        }
    }
}