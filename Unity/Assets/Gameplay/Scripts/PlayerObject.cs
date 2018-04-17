using System;
using System.Collections;
using System.Collections.Generic;
using BattleRobo;
using UnityEngine;
using Photon;

public class PlayerObject : PunBehaviour
{
    [SerializeField]
    private int id;
    [SerializeField]
    private int maxStackSize;
    [SerializeField] 
    private Sprite itemSprite;
    
    private Transform position;
    private PhotonView myPhotonView;

    public void Start()
    {
        myPhotonView = GetComponent<PhotonView>();
    }

    public Sprite GetSprite()
    {
        return itemSprite;
    }
    
    public int GetId()
    {
        return id;
    }

    public void Take()
    {
        myPhotonView.RequestOwnership();
        myPhotonView.RPC("Hide", PhotonTargets.AllViaServer);
    }

    [PunRPC]
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

    public void Destroy()
    {
        Destroy(this.gameObject);
    }
}