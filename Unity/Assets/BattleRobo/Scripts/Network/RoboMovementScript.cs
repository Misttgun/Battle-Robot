using System;
using Photon;
using UnityEngine;

namespace BattleRobo
{
    [RequireComponent(typeof(PhotonView))]
    public class RoboMovementScript : PunBehaviour
    {
        /// <summary>
        /// Aim sensitivity.
        /// </summary>
        [Header("Mouse Settings")]
        [SerializeField]
        private float aimSensitivity = 5f;

        /// <summary>
        /// Player run speed.
        /// </summary>[Header("Movement Settings")]
        [SerializeField]
        private float speed = 9.0f;

        /// <summary>
        /// Gravity applied to the player.
        /// </summary>
        [SerializeField]
        private float gravity = 20.0f;

        /// <summary>
        /// Speed at which the fuel decrease.
        /// </summary>
        [SerializeField]
        private float fuelDecreaseSpeed = 0.1f;

        /// <summary>
        /// Speed at which the fuel regenerate.
        /// </summary>
        [SerializeField]
        private float fuelRegenSpeed = 0.05f;

        /// <summary>
        /// Fly force for the jetpack.
        /// </summary>
        [SerializeField]
        private float flyForce = 100f;

        /// <summary>
        /// The CharacterController for the player.
        /// </summary>
        [Header("Required Components")]
        [SerializeField]
        private CharacterController controller;

        /// <summary>
        /// The robo head transform for the camera rotation.
        /// </summary>
        [SerializeField]
        private Transform roboHead;

        /// <summary>
        /// A reference to the current RoboLogi script.
        /// </summary>
        [SerializeField]
        private RoboLogicScript roboLogic;

        //movement variable
        private Vector3 moveDirection = Vector3.zero;
        private bool grounded;

        //fly variables
        public Vector3 fly;

        private float currentRot;

        private PlayerInput input;

        private float timer;

        private int currentTick;
        private const int BufferSize = 1024;
        private readonly PlayerState[] stateBuffer = new PlayerState[BufferSize]; // client stores predicted moves here
        private readonly PlayerInput[] inputBuffer = new PlayerInput[BufferSize]; // client stores predicted inputs here


        private void Start()
        {
            currentTick = 0;
        }

        private void FixedUpdate()
        {
            if (roboLogic.isInPause)
                return;

            timer += Time.fixedDeltaTime;

            if (PhotonNetwork.isMasterClient)
            {
                if (timer >= 0.1f)
                {
                    photonView.RPC("NetworkUpdate", PhotonTargets.Others, transform.position, transform.rotation, currentTick);
                    timer = 0f;
                }
            }

            if (photonView.isMine)
            {
                input.inputX = Input.GetAxis("Horizontal");
                input.inputY = Input.GetAxis("Vertical");
                input.jump = Input.GetButton("Jump");
                input.mouse.x = Input.GetAxisRaw("Mouse X");
                input.mouse.y = Input.GetAxisRaw("Mouse Y");

                int bufferSlot = currentTick % BufferSize;

                stateBuffer[bufferSlot].position = transform.position;
                stateBuffer[bufferSlot].rotation = transform.rotation;

                inputBuffer[bufferSlot] = input;

                Simulate(Time.fixedDeltaTime);

                photonView.RPC("UpdateServer", PhotonTargets.Others, currentTick, input.inputX, input.inputY, input.jump, input.mouse);

                ++currentTick;
            }
        }

        private void Update()
        {
            if (roboLogic.isInPause)
                return;

            if (roboLogic.isJumpingAudio)
            {
                //Stop the running sound if we are not grounded
                if (roboLogic.audioSource.isPlaying && roboLogic.audioSource.clip == roboLogic.audioClips[0])
                {
                    roboLogic.audioSource.Stop();
                }

                //Play the jetpack sound
                if (!roboLogic.audioSource.isPlaying)
                {
                    AudioManagerScript.Play3D(roboLogic.audioSource, roboLogic.audioClips[1], 1.15f);
                }
            }
            else
            {
                //Stop the jumping sound when we are falling
                if (roboLogic.audioSource.isPlaying && roboLogic.audioSource.clip == roboLogic.audioClips[1])
                {
                    roboLogic.audioSource.Stop();
                }
            }

            fly = Vector3.zero;
            if (input.jump && roboLogic.fuelAmount > 0f)
            {
                roboLogic.fuelAmount -= fuelDecreaseSpeed * Time.deltaTime;
                var consumedFuel = roboLogic.MaxFuelAmount - roboLogic.fuelAmount;

                if (roboLogic.fuelAmount >= 0.1f)
                {
                    // We override the base layer when the player is jumping
                    roboLogic.animator.SetLayerWeight(1, 1);

                    roboLogic.isJumpingAudio = true;

                    fly = Vector3.up * flyForce * consumedFuel;
                }
            }
            else
            {
                roboLogic.isJumpingAudio = false;
                roboLogic.fuelAmount += fuelRegenSpeed * Time.deltaTime;
                roboLogic.MaxFuelAmount = roboLogic.fuelAmount;
            }

            roboLogic.fuelAmount = Mathf.Clamp(roboLogic.fuelAmount, 0f, 1f);
        }

        private void LateUpdate()
        {
            if (roboLogic.isInPause)
                return;

            // Rotate the player on the X axis
            currentRot -= input.mouse.y * aimSensitivity;
            currentRot = Mathf.Clamp(currentRot, -60f, 60f);

            // Make the weapon loot in the same direction as the cam
            roboLogic.animator.SetFloat("AimAngle", currentRot);
            roboHead.transform.localEulerAngles = new Vector3(currentRot, 0f, 0f);
        }

        private void Simulate(float dt)
        {
            if (roboLogic.isInPause)
                return;

            // Limit the diagonal speed
            float inputModifyFactor = Math.Abs(input.inputX) > 0.0001f && Math.Abs(input.inputY) > 0.0001f ? .7071f : 1.0f;

            // Handle the movement
            if (grounded)
            {
                // Disable the jump layer when the player is on the ground
                roboLogic.animator.SetLayerWeight(1, 0);

                // Disable the thrusters when the player is not flying
                roboLogic.thrusters.SetActive(false);

                moveDirection = new Vector3(input.inputX * inputModifyFactor * speed, 0f, input.inputY * inputModifyFactor * speed);

                //Stop the jetpack sound if we are grounded
                if (roboLogic.audioSource.isPlaying && roboLogic.audioSource.clip == roboLogic.audioClips[1])
                {
                    roboLogic.audioSource.Stop();
                }

                //Play the run audio
                if (roboLogic.isMoovingAudio && !roboLogic.audioSource.isPlaying)
                {
                    AudioManagerScript.Play3D(roboLogic.audioSource, roboLogic.audioClips[0]);
                }

                // Animate the player for the ground animation
                roboLogic.animator.SetFloat("VelX", moveDirection.x * speed);
                roboLogic.animator.SetFloat("VelY", moveDirection.z * speed);

                moveDirection = transform.TransformDirection(moveDirection);

                // Jump!
                Jump();
            }
            else
            {
                moveDirection.x = input.inputX * speed * inputModifyFactor;
                moveDirection.z = input.inputY * speed * inputModifyFactor;

                // Animate the player for the ground animation
                roboLogic.animator.SetFloat("VelX", moveDirection.x * speed);
                roboLogic.animator.SetFloat("VelY", moveDirection.z * speed);

                // Disable the thrusters when the player is not flying
                roboLogic.thrusters.SetActive(true);

                moveDirection = transform.TransformDirection(moveDirection);

                // Jump!
                Jump();
            }

            // Rotate the player on the Y axis
            transform.rotation *= Quaternion.Euler(0, input.mouse.x * aimSensitivity, 0);

            // Apply gravity
            moveDirection.y -= gravity * dt;

            //Set the mooving bool
            roboLogic.isMoovingAudio = Math.Abs(moveDirection.x) > 0.0001f || Math.Abs(moveDirection.z) > 0.0001f;

            // Move the controller, and set grounded true or false depending on whether we're standing on something
            grounded = (controller.Move(moveDirection * dt) & CollisionFlags.Below) != 0;
        }

        private void Jump()
        {
            if (fly != Vector3.zero)
            {
                moveDirection.y = fly.y;
            }
        }

        [PunRPC]
        private void NetworkUpdate(Vector3 pos, Quaternion rot, int tick)
        {
            int bufferSlot = tick % BufferSize;

            if (!((pos - stateBuffer[bufferSlot].position).sqrMagnitude > 0.01f) && !(Quaternion.Dot(rot, stateBuffer[bufferSlot].rotation) < 0.99f))
                return;

            transform.position = pos;
            transform.rotation = rot;

            int rewindTick = tick;
            while (rewindTick < currentTick)
            {
                bufferSlot = rewindTick % BufferSize;
                stateBuffer[bufferSlot].position = transform.position;
                stateBuffer[bufferSlot].rotation = transform.rotation;
                input = inputBuffer[bufferSlot];

                Simulate(Time.fixedDeltaTime);

                ++rewindTick;
            }
        }

        [PunRPC]
        private void UpdateServer(int moveNumber, float inputX, float inputY, bool jump, Vector2 mouse)
        {
            input.inputX = inputX;
            input.inputY = inputY;
            input.jump = jump;
            input.mouse = mouse;

            Simulate(Time.fixedDeltaTime);

            currentTick = moveNumber + 1;

            int bufferSlot = currentTick % BufferSize;

            stateBuffer[bufferSlot].position = transform.position;
            stateBuffer[bufferSlot].rotation = transform.rotation;
            inputBuffer[bufferSlot] = input;
        }
    }
}