using System;
using System.Collections;
using System.Collections.Generic;
using BattleRobo;
using UnityEngine;

public class PlayerObject : MonoBehaviour
{
    [SerializeField]
    private int id;
    [SerializeField]
    private int maxStackSize;
    
    private Transform position;

    public void Start()
    {
        Debug.Log("START : Item is created");
    }
    
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<PlayerControllerScript>().GetInventory().AddNearItem(this);
    }
    
    private void OnTriggerExit(Collider other)
    {
        other.GetComponent<PlayerControllerScript>().GetInventory().RemoveNearItem(this);
    }
    
    public int GetId()
    {
        return id;
    }

    public void Hide()
    {
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;
    }

    public void Drop(Vector3 position)
    {
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<Transform>().position = position;
    }
    
}
