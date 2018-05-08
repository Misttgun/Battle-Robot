using System.Collections;
using UnityEngine;

namespace BattleRobo
{
    public class StormManagerScript : Photon.PunBehaviour
    {
        // reference to this script instance
        private static StormManagerScript Instance;

        [SerializeField]
        private float sizing;

        [SerializeField]
        private float waitTime;

        [SerializeField]
        private float stormTimer;

        [SerializeField]
        private LevelGeneratorScript mapGenerator;

        public int stormDmg = 2;

        private float stormSize; // à calculer avec la taille de la map generator
        private Vector3 size;
        private const float LerpTime = 1f;
        private float currentLerpTime;

        private float timer;

        private bool lerping;
        private bool startTimer;

        private float startSize;
        private float endSize;

        private bool stormActive;
        private float h;
        private int m;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        private void Start()
        {
            startTimer = false;
            StormTransform(); // donner la taille de départ de la zone
        }

        private void Update()
        {
            if (PhotonNetwork.isMasterClient && !startTimer)
            {
                timer += Time.deltaTime;
                if (timer >= 2f)
                {
                    //start the storm countdown at the same time on all client
                    photonView.RPC("StartTimerRPC", PhotonTargets.AllViaServer);
                }
            }

            if (startTimer)
            {
                if (stormTimer <= 0)
                {
                    StartCoroutine(StormManageScale());


                    stormTimer = 0f;
                }
                else
                {
                    stormTimer -= Time.deltaTime;
                }
            }

            if (lerping)
            {
                currentLerpTime += Time.deltaTime;
                if (currentLerpTime > LerpTime)
                {
                    currentLerpTime = LerpTime;
                    lerping = false;
                }

                float ratio = currentLerpTime / LerpTime;
                stormSize = Mathf.Clamp(Mathf.Lerp(startSize, endSize, ratio), 10f, 1000f); //1000f peut etre changer
                transform.localScale = new Vector3(stormSize, stormSize * h, stormSize);
            }
        }

        private IEnumerator StormManageScale()
        {
            stormActive = true;
            while (stormActive)
            {
                startSize = stormSize;
                endSize = stormSize - sizing;
                lerping = true;
                currentLerpTime = 0;

                yield return new WaitForSeconds(waitTime);
            }
        }

        private void StormTransform() //taille de la storm
        {
            m = mapGenerator.getMapMainSize();
            h = mapGenerator.getHeight() / 6;
            stormSize = m * 3 / 2.2f;
            float stormCenterPos = (float) m / 2;
            transform.localScale = new Vector3(stormSize, stormSize * h, stormSize);
            transform.position = new Vector3(stormCenterPos, h / 2, stormCenterPos);
        }

        private void OnTriggerExit(Collider other)
        {
            other.GetComponent<RoboController>().inStorm = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            other.GetComponent<RoboController>().inStorm = false;
        }

        //called on the master client when the strom start to move
        [PunRPC]
        private void StartTimerRPC()
        {
            startTimer = true;
        }

        /// <summary>
        /// Returns a reference to this script instance.
        /// </summary>
        public static StormManagerScript GetInstance()
        {
            return Instance;
        }

        /// <summary>
        /// Returns the networked storm timer of the room out of properties.
        /// </summary>
        public float GetStormTimer()
        {
            return stormTimer;
        }
    }
}