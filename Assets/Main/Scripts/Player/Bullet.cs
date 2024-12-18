using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviour
{
    public float life = 3;

    private Enemy _enemy;

    private void Awake()
    {
        Destroy(gameObject, life);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            _enemy = collision.gameObject.GetComponent<Enemy>();
            _enemy.Dead();
        }
        Destroy(gameObject);
    }
}
