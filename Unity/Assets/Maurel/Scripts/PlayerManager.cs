using Maurel.BattleRobo.Networking;
using UnityEngine;

namespace Maurel.BattleRobo.Player
{
    public class PlayerManager : Photon.PunBehaviour
    {
        #region Public Variables
        
        [Tooltip("The current health of our player")]
        public float health = 1f;

        #endregion

        #region Private Variables
        
        [Tooltip("The Beams GameObject to control")]
        [SerializeField]
        public GameObject beams;

        //True, when the user is firing
        private bool isFiring;

        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            if (beams == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> Beams Reference.", this);
            }
            else
            {
                beams.SetActive(false);
            }
        }

        private void Update()
        {
            ProcessInputs();

            // trigger Beams active state 
            if (beams != null && isFiring != beams.GetActive())
            {
                beams.SetActive(isFiring);
            }

            if (health <= 0f)
            {
                GameManager.Instance.LeaveRoom();
            }
        }
        
        private void OnTriggerEnter(Collider other) 
        {
            // we don't do anything if we are not the local player.
            if (!photonView.isMine)
            {
                return;
            }
 
            // We are only interested in Beamers
            // we should be using tags but for the sake of distribution, let's simply check by name.
            if (!other.name.Contains("Beam"))
            {
                return;
            }
 
            health -= 0.1f;
        }
 
        private void OnTriggerStay(Collider other) 
        {
            // we don't do anything if we are not the local player.
            if (!photonView.isMine) 
            {
                return;
            }
 
            // We are only interested in Beamers
            // we should be using tags but for the sake of distribution, let's simply check by name.
            if (!other.name.Contains("Beam"))
            {
                return;
            }
 
            // we slowly affect health when beam is constantly hitting us, so player has to move to prevent death.
            health -= 0.1f*Time.deltaTime; 
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Processes the inputs. Maintain a flag representing when the user is pressing Fire.
        /// </summary>
        private void ProcessInputs()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (!isFiring)
                {
                    isFiring = true;
                }
            }

            if (Input.GetButtonUp("Fire1"))
            {
                if (isFiring)
                {
                    isFiring = false;
                }
            }
        }

        #endregion
    }
}