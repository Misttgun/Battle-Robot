using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace BattleRobo
{
	/// <summary>
	/// This class extends Photon's PhotonPlayer object by custom properties.
	/// Provides several methods for setting and getting variables out of them.
	/// </summary>
	public class PlayerStatsScript
	{
		//keys for saving and accessing values in custom properties Hashtable
		public const string health = "health";
		public const string shield = "shield";
		public const string fuel = "fuel";
		public const string kills = "kills";


		/// <summary>
		/// Returns the networked player nick name.
		/// </summary>
		public string GetName()
		{
				return PhotonNetwork.player.NickName;
		}

		/// <summary>
		/// Returns the networked health value of the player out of properties.
		/// </summary>
		public int GetHealth()
		{
			return System.Convert.ToInt32(PhotonNetwork.player.CustomProperties[health]);
		}

		/// <summary>
		/// Synchronizes the health value of the player for all players via properties.
		/// </summary>
		public void SetHealth(int value)
		{
			PhotonNetwork.player.SetCustomProperties(new Hashtable { { health, (byte)value } });
		}

		/// <summary>
		/// Returns the networked shield value of the player out of properties.
		/// </summary>
		public int GetShield()
		{
			return System.Convert.ToInt32(PhotonNetwork.player.CustomProperties[shield]);
		}

		/// <summary>
		/// Synchronizes the shield value of the player for all players via properties.
		/// </summary>
		public void SetShield(int value)
		{
			PhotonNetwork.player.SetCustomProperties(new Hashtable { { shield, (byte)value } });
		}

		/// <summary>
		/// Returns the networked fuel value of the player out of properties.
		/// </summary>
		public float GetFuel()
		{
			return System.Convert.ToSingle(PhotonNetwork.player.CustomProperties[fuel]);
		}

		/// <summary>
		/// Synchronizes the fuel value of the player for all players via properties.
		/// </summary>
		public void SetFuel(float value)
		{
			PhotonNetwork.player.SetCustomProperties(new Hashtable { { fuel, (byte)value } });
		}

		/// <summary>
		/// Returns the networked kills value of the player out of properties.
		/// </summary>
		public int GetKills()
		{
			return System.Convert.ToInt32(PhotonNetwork.player.CustomProperties[fuel]);
		}

		/// <summary>
		/// Add one to the kills value of the player for all players via properties.
		/// </summary>
		public void AddKills()
		{
			int kills = GetKills();
			kills++;

			SetKills(kills);
		}

		/// <summary>
		/// Synchronizes the kills value of the player for all players via properties.
		/// </summary>
		public void SetKills(int value)
		{
			PhotonNetwork.player.SetCustomProperties(new Hashtable { { fuel, (byte)value } });
		}

		/// <summary>
		/// Clears all networked variables of the player via properties in one instruction.
		/// </summary>
		public void Clear()
		{
			PhotonNetwork.player.SetCustomProperties(new Hashtable { { fuel, (byte)0 },
                                                         { health, (byte)0 },
														 { kills, (byte)0 },
                                                         { shield, (byte)0 } });
		}
	}
}
