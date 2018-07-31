using System;
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
        private float LerpTime = 0.5f;
        
        [SerializeField]
        private float stormSize;

        public int stormDmg = 2;
        
        private Vector3 size;
        private float currentLerpTime;

        private float timer;

        private bool lerping;
        private bool startTimer;

        private float startSize;
        private float endSize;

        private bool stormActive;

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

        private void FixedUpdate()
        {
            if (GameManagerScript.ready && !startTimer)
            {
                //start the storm countdown at the same time on all client
                photonView.RPC("StartTimerRPC", PhotonTargets.AllViaServer);
            }

            if (GameManagerScript.GetInstance().IsGamePause())
                return;

            if (Math.Abs(stormSize - sizing) < 0.00001f)
            {
                stormActive = false;
            }

            if (startTimer && stormActive)
            {
                if (stormTimer > 0)
                {
                    stormTimer -= Time.fixedDeltaTime;

                    timer = stormTimer;
                }

                if (timer <= 0)
                {
                    StormManageScale();

                    timer = waitTime;
                }
                else
                {
                    timer -= Time.fixedDeltaTime;
                }
            }

            if (lerping)
            {
                currentLerpTime += Time.fixedDeltaTime;
                if (currentLerpTime > LerpTime)
                {
                    currentLerpTime = LerpTime;
                    lerping = false;
                }

                float ratio = currentLerpTime / LerpTime;
                stormSize = Mathf.Clamp(Mathf.Lerp(startSize, endSize, ratio), 0, 10000f); //1000f peut etre changer
                transform.localScale = new Vector3(stormSize, stormSize , stormSize);
            }
        }

        private void StormManageScale()
        {
            startSize = stormSize;
            endSize = GameManagerScript.GetInstance().IsGamePause() ? stormSize : stormSize - sizing;
            lerping = true;
            currentLerpTime = 0;
        }

        private void StormTransform() //taille de la storm
        {
            transform.localScale = new Vector3(stormSize, stormSize, stormSize);
            //TODO Enlever les valeur en dur
            transform.position = new Vector3(200f, 0, 200f);
        }

        private void OnTriggerExit(Collider other)
        {
            other.GetComponent<HealthScript>().inStorm = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            other.GetComponent<HealthScript>().inStorm = false;
        }

        //called on the master client when the strom start to move
        [PunRPC]
        private void StartTimerRPC()
        {
            startTimer = true;
            stormActive = true;
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