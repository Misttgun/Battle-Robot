using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleRobo
{
    public class ConsommableHolderScript : MonoBehaviour
    {

        public ConsommableScript currentConsommable;

        [SerializeField]
        public List<BattleRobo.ConsommableScript> consommableList;
        
        [SerializeField]
        private PhotonView playerPhotonView;

        public void SetConsommable(ConsommableScript consommableScript)
        {
            int consommableId = consommableScript ? consommableScript.GetId() : -1;

            playerPhotonView.RPC("EquipConsommableRPC", PhotonTargets.All, -1);
        }

        public void EquipConsommable(int consommableIndex)
        {
            var consommable = consommableIndex != -1 ? consommableList[consommableIndex] : null;
            currentConsommable = consommable;
        }
    }
}

