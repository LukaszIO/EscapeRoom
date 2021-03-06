﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class GOEvent : UnityEvent<GameObject>
{
}

public class DraggableItem : Photon.PunBehaviour
{
    public float UseDistance = UsableTargeter.DefaultItemUseDistance;

    public bool freeMovement = true;
    public bool IsPuzzleElement = false;
    public string PuzzleElementId;
    [HideInInspector]
    public Vector3 defaultPosition;
    public GOEvent OnDetached;
    public GOEvent OnAttached;
    void Start()
    {
        defaultPosition = transform.position;
    }

    void FixedUpdate()
    {
        if(transform.position.y < -10000)
        {
            transform.position = defaultPosition;
        }
    }
    public void ChangeOwner(int playerID)
    {
        photonView.TransferOwnership(playerID);
    }

    public void AttachToPlayer(int viewID)
    {
        photonView.RPC("RpcAttachToPlayer", PhotonTargets.All, viewID);
    }

    public void DetachFromPlayer()
    {
        photonView.RPC("RpcDetachFromPlayer", PhotonTargets.All);
    }

    [PunRPC]
    void RpcAttachToPlayer(int viewID)
    {
        if (freeMovement)
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            gameObject.layer = LayerMask.NameToLayer("NoPlayerCollision");
            transform.SetParent(PhotonView.Find(viewID).transform);
        }
        else
        {
            GetComponent<ConfigurableJoint>().connectedBody = PhotonView.Find(viewID).GetComponent<Rigidbody>();
        }
        OnAttached.Invoke(this.gameObject);
    }

    [PunRPC]
    void RpcDetachFromPlayer()
    {

        if (freeMovement)
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            gameObject.layer = LayerMask.NameToLayer("PickableItem");
            transform.SetParent(null);
        }
        else
        {
            GetComponent<ConfigurableJoint>().connectedBody = null;
        }
        OnDetached.Invoke(this.gameObject);
    }
}
