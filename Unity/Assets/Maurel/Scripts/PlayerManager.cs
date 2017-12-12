using UnityEngine;
using GameManager = Maurel.BattleRobo.Networking.GameManager;

namespace Maurel.BattleRobo.Player
{
    [RequireComponent(typeof(PlayerCamera))]
    public class PlayerManager : Photon.PunBehaviour, IPunObservable
    {
        #region Public Variables

        [Tooltip("The current health of our player")]
        public float health = 1f;

        [Tooltip("The local player instance. Used to know if the local player is represented in the Scene")]
        public static GameObject localPlayerInstance;

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
                Debug.LogError("Missing Beams Reference.", this);
            }
            else
            {
                beams.SetActive(false);
            }

            // #Important: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
            if (photonView.isMine)
            {
                localPlayerInstance = gameObject;
            }
            // #Critical: we flag as don't destroy on load so that instance survives level synchronization.
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            PlayerCamera pCamera = GetComponent<PlayerCamera>();

            if (photonView.isMine)
            {
                pCamera.OnStartFollowing();
            }

//            UnityEngine.SceneManagement.SceneManager.sceneLoaded += (scene, loadingMode) =>
//            {
//                CalledOnLevelWasLoaded(scene.buildIndex);
//            };
        }

        private void Update()
        {
            if (photonView.isMine)
            {
                ProcessInputs();
            }

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
            health -= 0.1f * Time.deltaTime;
        }

//        private void CalledOnLevelWasLoaded(int level)
//        {
//            // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
//            if (!Physics.Raycast(transform.position, Vector3.down, 5f))
//            {
//                transform.position = new Vector3(0f, 5f, 0f);
//            }
//        }

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

        #region IPunObservable Methods

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.isWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(isFiring);
                stream.SendNext(health);
            }
            else
            {
                // Network player, receive data
                isFiring = (bool) stream.ReceiveNext();
                health = (float) stream.ReceiveNext();
            }
        }

        #endregion
    }
}