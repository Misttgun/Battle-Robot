using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormPlayerController : MonoBehaviour
{
    [SerializeField] private int life=10;
    [SerializeField] private bool InTheStorm;
    
    public void StormApplyDmg(int Dmg)
    {
        StartCoroutine(StormManageDmg(Dmg));
    }

    public void StormStopDmg()
    {
        InTheStorm = false;
    }

    IEnumerator StormManageDmg(int Dmg)
    {
        InTheStorm = true;
        while (InTheStorm)
        {
            yield return new WaitForSeconds(1f);
            life-=Dmg;
            if (life <= 0)
            {
                life = 0;
                InTheStorm = false;
            } 
        }
    }

}
