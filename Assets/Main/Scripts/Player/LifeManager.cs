using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class LifeManager : MonoBehaviourPunCallbacks
{
    public int maxLife = 3;

    [SerializeField]
    private int currentLife;

    [SerializeField]
    private GameManager GM;

    private float time = 3;

    private bool canHit;

    private PhotonView PV;

    [SerializeField]
    public Image currentLifeI;

    public int _currentLife
    {
        get
        {
            return currentLife;
        }
        set
        {
            currentLife = value;
        }
    }

    private void Awake()
    {
        currentLife = maxLife;
        canHit = true;
        GM = FindObjectOfType<GameManager>();
        PV = GetComponent<PhotonView>();
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

    [PunRPC]
    public void RPC_TakeDamage(int _damage)
    {
        if (canHit)
        {
            currentLife -= _damage;
            Debug.LogError(_currentLife);
            canHit = false;
            if (currentLife <= 0)
            {
                PlayerDead();
            }
            currentLifeI.fillAmount = (float)currentLife / 3f;
        }
    }

    [PunRPC]
    public void RPC_RegenerationLife(int _regeneration)
    {
        if (PV.IsMine && currentLife <= 2)
        {
            currentLife += _regeneration;
            Debug.LogError(_currentLife);
        }
    }

    private void PlayerDead()
    {
        GM.LoseGame();
    }

}
