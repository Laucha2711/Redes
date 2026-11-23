using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviour
{
    public float life = 3;

    private Enemy _enemy;
    private PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        Destroy(gameObject, life);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            _enemy = collision.gameObject.GetComponent<Enemy>();
            _enemy.Dead();
        }
        else if (collision.gameObject.tag == "Player" && PV.IsMine)
        {
            LifeManager lifeManager = collision.gameObject.GetComponent<LifeManager>();
            if (lifeManager != null && !lifeManager.photonView.IsMine)
            {
                lifeManager.photonView.RPC(nameof(LifeManager.RPC_TakeDamage), RpcTarget.All, 1);
            }
        }

        Destroy(gameObject);
    }
}
