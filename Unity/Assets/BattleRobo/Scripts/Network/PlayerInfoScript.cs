using System.Runtime.Remoting.Messaging;
using System.Security.Principal;
using UnityEngine;

namespace BattleRobo.Scripts.Network
{
    public class PlayerInfoScript : MonoBehaviour
    {
        //reference to this script instance
        private static PlayerInfoScript instance;
        
        /// <summary>
        /// Store the player token used by the master client to set the stats of the player in the database
        /// </summary>
        private string dbToken;
        
        public void SetDBToken(string token)
        {
            dbToken = token;
        }

        public string GetDBToken()
        {
            return dbToken;
        }

        // Sets the instance reference
        private void Awake()
        {
            if (instance != null)
                return;

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public static PlayerInfoScript GetInstance()
        {
            return instance;
        }
    }
}