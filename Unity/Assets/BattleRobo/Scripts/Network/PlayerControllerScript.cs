using UnityEngine;
using BattleRobo.Networking;

namespace BattleRobo.Core
{
    public class PlayerControllerScript : Photon.PunBehaviour
    {
        [SerializeField] private float roboSpeed = 25f;

        [SerializeField] private float aimSensitivity = 5f;

        [SerializeField] private float aimSensitivityY = 10f;

        [SerializeField] private float flyForce = 100f;

        [SerializeField] private float fuelDecreaseSpeed = 0.1f;

        [SerializeField] private float fuelRegenSpeed = 0.05f;

        [SerializeField] private Transform roboChest;

        [SerializeField] private GameObject playerUI;

        [SerializeField] private Rigidbody roboRb;

        [SerializeField] private GameObject cam;

        private Vector3 roboVelocity;
        private Vector3 roboRotY;

        private float roboRotUD;
        private float currentRot;
        private Vector3 fly;

        private float health = 100f;
        private float fuelAmount = 1f;

        public bool IsDead { get; private set; }

        public float Health
        {
            get { return health; }
        }

        public float FuelAmount
        {
            get { return fuelAmount; }
        }


        private void Start()
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;

            if (cam != null)
            {
                if (photonView.isMine)
                {
                    var spawns = GameManagerScript.Instance.spawnPoints;
                    
                    cam.SetActive(true);
                    IsDead = false;
                    playerUI.SetActive(true);
                    
                    transform.position = spawns[Random.Range(0, spawns.Length)].position;
                }
            }
        }

        private void Update()
        {
            if (!photonView.isMine && PhotonNetwork.connected) return;

            // Cursor lock
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            //Fly
            fly = Vector3.zero;
            if (Input.GetButton("Jump") && fuelAmount > 0f)
            {
                fuelAmount -= fuelDecreaseSpeed * Time.deltaTime;
                if (fuelAmount >= 0.04f)
                {
                    fly = Vector3.up * flyForce;
                }
            }
            else
            {
                fuelAmount += fuelRegenSpeed * Time.deltaTime;
            }

            fuelAmount = Mathf.Clamp(fuelAmount, 0f, 1f);

            if (transform.position.y <= 0f)
            {
                photonView.RPC("TakeDamage", PhotonTargets.All, 110f);
                transform.position = new Vector3(0, 100, 0);
            }
        }

        private void FixedUpdate()
        {
            if (!photonView.isMine && PhotonNetwork.connected) return;

            Vector3 movX = transform.right * Input.GetAxisRaw("Horizontal");
            Vector3 movY = transform.forward * Input.GetAxisRaw("Vertical");

            roboVelocity = (movX + movY).normalized * roboSpeed;

            // Souris (Aim)
            roboRotUD = Input.GetAxisRaw("Mouse Y") * aimSensitivity;

            if (roboVelocity != Vector3.zero)
            {
                roboRb.MovePosition(roboRb.position + roboVelocity * Time.fixedDeltaTime);
            }

            roboRb.MoveRotation(roboRb.rotation * Quaternion.Euler(0,
                                    Input.GetAxisRaw("Mouse X") * aimSensitivityY,
                                    0));

            currentRot -= roboRotUD;
            currentRot = Mathf.Clamp(currentRot, -70f, 70f);
            roboChest.transform.localEulerAngles = new Vector3(0f, 0f, -currentRot);

            if (fly != Vector3.zero)
            {
                Jump(fly);
            }
        }

        private void Jump(Vector3 jumpVec)
        {
            roboRb.AddForce(jumpVec * Time.fixedDeltaTime, ForceMode.Impulse);
        }

        [PunRPC]
        public void TakeDamage(float amount)
        {
            health -= amount;

            if (health <= 0f)
            {
                health = 0f;
                // player is dead
                Die();
            }
        }


        private void Die()
        {
            var photonId = photonView.instantiationId;

            if (photonId == 0)
            {
                Destroy(gameObject);
            }
            else
            {
                GameManagerScript.Instance.alivePlayerNumber--;
                if (!photonView.isMine)
                {
                    return;
                }
                IsDead = true;
                GameManagerScript.Instance.ShowGameOverScreen("You died... Feels bad man !!");
            }
        }
    }
}