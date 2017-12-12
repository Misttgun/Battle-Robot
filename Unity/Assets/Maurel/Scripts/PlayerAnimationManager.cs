using UnityEngine;

namespace Maurel.BattleRobo.Player
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimationManager : Photon.MonoBehaviour
    {
        #region Public Variables

        /// <summary>
        /// Damping variable turn smooth the player rotation.
        /// </summary>
        public float directionDampTime = .25f;

        #endregion

        #region Private Variables

        /// <summary>
        /// The player animator.
        /// </summary>
        private Animator animator;

        #endregion

        #region MonoBehaviour Callbacks

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (!photonView.isMine && PhotonNetwork.connected)
            {
                return;
            }
            
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            if (v < 0)
            {
                v = 0;
            }

            animator.SetFloat("Speed", h * h + v * v);
            animator.SetFloat("Direction", h, directionDampTime, Time.deltaTime);

            // deal with Jumping
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            
            // only allow jumping if we are running.
            if (stateInfo.IsName("Base Layer.Run"))
            {
                // When using trigger parameter
                if (Input.GetButtonDown("Jump"))
                {
                    animator.SetTrigger("Jump");
                }
            }
        }

        #endregion
    }
}