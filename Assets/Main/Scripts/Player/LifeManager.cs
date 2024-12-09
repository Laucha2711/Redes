using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LifeManager : MonoBehaviourPunCallbacks
{
    public int maxLife = 3;
    public int currentLife;

    [SerializeField]
    private GameManager GM;

    private float time = 3;

    private bool canHit;

    private void Awake()
    {
        currentLife = maxLife;
        canHit = true;
        GM = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            if (!canHit)
            {
                time = time - 1 * Time.deltaTime;

                if (time <= 0)
                {
                    canHit = true;
                    time = 5;
                }
            }
        }
    }

    /*public void GetDamage(int _damage)
    {
        if (photonView.IsMine)
        {
            if (canHit)
            {
                currentLife -= _damage;
                Debug.Log(currentLife);
                canHit = false;
                if (currentLife <= 0)
                {
                    PlayerDead();
                }
            }
        }
    }
    */

    public void RegenerationLife(int _regeneration)
    {
        if (photonView.IsMine)
        {
            currentLife += _regeneration;
            Debug.Log(currentLife);
        }
    }

    private void PlayerDead()
    {
        GM.LoseGame();
    }

    [PunRPC]
    public void RPC_TakeDamage(int _damage)
    {
        if (canHit)
        {
            currentLife -= _damage;
            Debug.Log(currentLife);
            canHit = false;
            if (currentLife <= 0)
            {
                PlayerDead();
            }
        }
    }
}
