using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class LifeManager : MonoBehaviourPunCallbacks, IPunObservable
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
        if (!PV.IsMine)
        {
            return;
        }

        if (canHit)
        {
            currentLife -= _damage;
            canHit = false;
            if (currentLife <= 0)
            {
                PlayerDead();
            }
            currentLifeI.fillAmount = (float)currentLife / maxLife;
        }
    }

    [PunRPC]
    public void RPC_RegenerationLife(int _regeneration)
    {
        if (PV.IsMine && currentLife < maxLife)
        {
            currentLife += _regeneration;
            if (currentLife > maxLife)
            {
                currentLife = maxLife;
            }
            currentLifeI.fillAmount = (float)currentLife / maxLife;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentLife);
        }
        else
        {
            currentLife = (int)stream.ReceiveNext();
            if (currentLifeI != null)
            {
                currentLifeI.fillAmount = (float)currentLife / maxLife;
            }
        }
    }

    private void PlayerDead()
    {
        GM.LoseGame();
    }

}
