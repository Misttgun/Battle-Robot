using System.Collections.Generic;
using UnityEngine;

namespace BattleRobo
{
    public class ConsommableHolderScript : MonoBehaviour
    {

        public ConsommableScript currentConsommable;

        [SerializeField]
        public List<ConsommableScript> consommableList;
        
        [SerializeField]
        private PhotonView playerPhotonView;

        public void SetConsommable(ConsommableScript consommableScript)
        {
            int consommableId = consommableScript ? consommableScript.GetId() : -1;
            playerPhotonView.RPC("EquipConsommableRPC", PhotonTargets.All, consommableId);
        }

        
        public void EquipConsommable(int consommableIndex)
        {
            var consommable = consommableIndex != -1 ? consommableList[consommableIndex] : null;
            currentConsommable = consommable;
        }
    }
}

