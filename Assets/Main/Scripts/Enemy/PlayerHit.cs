using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerHit : MonoBehaviour
{
    private LifeManager lm;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            lm = other.gameObject.GetComponent<LifeManager>();
            lm.photonView.RPC(nameof(LifeManager.RPC_TakeDamage), RpcTarget.All, 1);
        }
    }   
}
