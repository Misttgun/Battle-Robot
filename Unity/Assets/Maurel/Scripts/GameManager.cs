﻿using UnityEngine;
using UnityEngine.SceneManagement;

namespace Maurel.BattleRobo.Networking
{
    public class GameManager : Photon.PunBehaviour
    {
        
        #region Public Variables
        
        public static GameManager Instance;
        
        #endregion
        
        #region Monobehaviour Callbacks

        private void Start()
        {
            Instance = this;
        }

        #endregion
        
        #region Photon Callbacks
 
        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }
        
        /// <summary>
        /// Called when a player join the room. We load the launcher scene if we are the master client.
        /// </summary>
        public override void OnPhotonPlayerConnected(PhotonPlayer other)
        {
            Debug.Log("OnPhotonPlayerConnected() " + other.NickName); // not seen if you're the player connecting
 
 
            if (PhotonNetwork.isMasterClient) 
            {
                Debug.Log("BattleRobo/GameManager: OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected
 
 
                LoadArena();
            }
        }
 
 
        /// <summary>
        /// Called when a player have left the room. We load the launcher scene if we are the master client.
        /// </summary>
        public override void OnPhotonPlayerDisconnected(PhotonPlayer other)
        {
            Debug.Log("BattleRobo/GameManager: OnPhotonPlayerDisconnected() " + other.NickName); // seen when other disconnects
 
 
            if (PhotonNetwork.isMasterClient) 
            {
                Debug.Log("BattleRobo/GameManager: OnPhotonPlayerDisonnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected
 
 
                LoadArena();
            }
        }
 
        #endregion
        
        #region Private Methods
 
        /// <summary>
        /// Called when we need to load the start scene for the player.
        /// </summary>
        void LoadArena()
        {
            if (!PhotonNetwork.isMasterClient) 
            {
                Debug.LogError("BattleRobo/GameManager: Trying to Load a level but we are not the master Client");
            }
            Debug.Log("BattleRobo/GameManager: Loading the Start Scene");
            PhotonNetwork.LoadLevel("StartScene");
        }
 
 
        #endregion
 
        #region Public Methods
 
        /// <summary>
        /// Called when a player want to quit the game and click on the quit button.
        /// </summary>
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }
 
        #endregion  
    }
}