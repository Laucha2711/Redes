using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Grenade : MonoBehaviourPunCallbacks
{
    public GameObject SmallExplosion;
    public float delay = 3f;

    public float explosionForce = 10f;
    public float radius = 20f;

    void Start()
    {
        Invoke("Explode", delay);
    }

    private void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach(Collider near in colliders)
        {
            Rigidbody rig = near.GetComponent<Rigidbody>();

            if (rig != null)
                rig.AddExplosionForce(explosionForce, transform.position, radius, 1f, ForceMode.Impulse);

            Enemy _enemy = near.GetComponent<Enemy>();
            if (_enemy != null)
            {
                _enemy.Dead();
            }
        }

        if (photonView.IsMine)
        {
            PhotonNetwork.Instantiate("SmallExplosion", transform.position, transform.rotation);
            if (gameObject != null)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }

    }
}
