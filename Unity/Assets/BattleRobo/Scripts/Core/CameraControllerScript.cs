using UnityEngine;

namespace BattleRobo
{
    public class CameraControllerScript : MonoBehaviour
    {
        /// <summary>
        /// The target tranform the camera is going to follow.
        /// </summary>
        [SerializeField] private Transform target;
        
        /// <summary>
        /// The player script.
        /// </summary>
        //[SerializeField] private RoboControllerScript playerScript;
        
        /// <summary>
        /// The player script.
        /// </summary>
        [SerializeField] private PlayerControllerScript playerScript;

        /// <summary>
        /// The camera transform.
        /// </summary>
        private Transform camTransform;

        /// <summary>
        /// The distance between the camera and the target.
        /// </summary>
        public float distance = 5f;

        private Vector3 camDestination;

        //on calcule le mask pour le lancer du rayon
        private const int playerMask = 1 << 8;

        private const int envMask = 1 << 10;
        private const int combinedMask = playerMask | envMask;

        private void Start()
        {
            camTransform = transform;
        }

        private void LateUpdate()
        {
            camDestination = target.position + distance * -camTransform.forward;

            RaycastHit hit;

            Debug.DrawLine(target.position, camDestination, Color.green);

            if (Physics.Linecast(target.position, camDestination, out hit, combinedMask))
            {
                var hitPoint = new Vector3(hit.point.x, hit.point.y,
                    hit.point.z);
                camDestination = new Vector3(hitPoint.x + hit.normal.x * 0.2f, hitPoint.y, hit.point.z + hit.normal.z * 0.2f);
            }

            camTransform.position = camDestination;
        }

        private void FixedUpdate()
        {
            camTransform.rotation = target.rotation;
            camTransform.localEulerAngles = new Vector3(playerScript.currentRot, 0f, 0f);
        }
    }
}