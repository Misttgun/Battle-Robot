using System.Configuration;
using UnityEngine;

namespace BattleRobo
{
    public class ConsommableScript : MonoBehaviour
    {
        [SerializeField]
        private Consommable currentConsommable;

        [SerializeField]
        private Animator playerAnimator;

        //public AudioClip consommableSound;

        [SerializeField]
        private PhotonView playerPhotonView;

        private const int layerMask = 1 << 8;

        private void Awake()
        {
            
        }

        private void Update()
        {
            if (!playerPhotonView) //player photon view is null
                return;

            playerAnimator.SetLayerWeight(2, 1);
        }

        /// <summary>
        /// Fire the gun and deals the gun damage
        /// </summary>
        public void Use(int playerID)
        {
            if (!PhotonNetwork.isMasterClient)
                return;
        }

        public int GetId()
        {
            return currentConsommable.consommableId;
        }

        public int GetPlayerObjectType()
        {
            return 1;
        }
    }
}