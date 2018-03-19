namespace BattleRobo
{
	/// <summary>
	/// List of all keys saved on the user's device, be it for settings or selections.
	/// </summary>
	public class PrefsKeys
	{
		/// <summary>
		/// PlayerPrefs key for player name: UserXXXX
		/// </summary>
		public const string playerName = "playerName";

		/// <summary>
		/// PlayerPrefs key for background music state: true/false
		/// </summary>
		public const string playMusic = "playMusic";

		/// <summary>
		/// PlayerPrefs key for global audio volume: 0-1 range
		/// </summary>
		public const string appVolume = "appVolume";

		/// <summary>
		/// PlayerPrefs key for selected player model: 0/1/2 etc.
		/// </summary>
		public const string activeTank = "activeRobot";
	}
}
