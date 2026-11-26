using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class PickUp : MonoBehaviourPun
{
    private const int ThrowableLayer = 6;
    private const int MedicKitLayer = 7;
    private const float ClaimGracePeriod = 0.2f;

    private class PickupClaim
    {
        public int actorNumber;
        public int serverTimestamp;
    }

    private readonly List<PickupClaim> pendingClaims = new List<PickupClaim>();
    private PhotonView itemView;
    private bool claimed;
    private bool resolvingClaim;

    private void Awake()
    {
        itemView = GetComponent<PhotonView>();
    }

    private void OnTriggerEnter(Collider other)
    {
        PhotonView playerView = other.GetComponentInParent<PhotonView>();
        if (playerView == null || !playerView.IsMine || !playerView.CompareTag("Player"))
        {
            return;
        }

        LifeManager lifeManager = playerView.GetComponent<LifeManager>();
        if (CompareTag("Armor") && (lifeManager == null || lifeManager._currentLife > 5))
        {
            return;
        }

        if (!IsPickup())
        {
            return;
        }

        if (!PhotonNetwork.InRoom)
        {
            GrantPickup(playerView.gameObject);
            Destroy(gameObject);
            return;
        }

        itemView.RPC(nameof(RPC_RequestPickup), RpcTarget.MasterClient, PhotonNetwork.ServerTimestamp);
    }

    [PunRPC]
    public void RPC_RequestPickup(int serverTimestamp, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient || claimed || info.Sender == null)
        {
            return;
        }

        int actorNumber = info.Sender.ActorNumber;
        foreach (PickupClaim claim in pendingClaims)
        {
            if (claim.actorNumber == actorNumber)
            {
                return;
            }
        }

        pendingClaims.Add(new PickupClaim
        {
            actorNumber = actorNumber,
            serverTimestamp = serverTimestamp
        });

        if (!resolvingClaim)
        {
            StartCoroutine(ResolvePickupClaims());
        }
    }

    private IEnumerator ResolvePickupClaims()
    {
        resolvingClaim = true;
        yield return new WaitForSeconds(ClaimGracePeriod);

        if (!PhotonNetwork.IsMasterClient || claimed || pendingClaims.Count == 0)
        {
            pendingClaims.Clear();
            resolvingClaim = false;
            yield break;
        }

        PickupClaim winner = pendingClaims[0];
        for (int i = 1; i < pendingClaims.Count; i++)
        {
            PickupClaim claim = pendingClaims[i];
            if (unchecked(claim.serverTimestamp - winner.serverTimestamp) < 0)
            {
                winner = claim;
            }
        }

        claimed = true;
        itemView.RPC(nameof(RPC_GrantPickup), RpcTarget.All, winner.actorNumber);
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    public void RPC_GrantPickup(int actorNumber)
    {
        if (PhotonNetwork.LocalPlayer == null || PhotonNetwork.LocalPlayer.ActorNumber != actorNumber)
        {
            return;
        }

        PhotonView playerView = FindPlayerView(actorNumber);
        if (playerView == null)
        {
            Debug.LogError("No encontre el jugador que agarro el item.");
            return;
        }

        GrantPickup(playerView.gameObject);
    }

    private bool IsPickup()
    {
        return gameObject.layer == ThrowableLayer ||
            gameObject.layer == MedicKitLayer ||
            CompareTag("Armor");
    }

    private PhotonView FindPlayerView(int actorNumber)
    {
        PhotonView[] views = FindObjectsOfType<PhotonView>();
        foreach (PhotonView view in views)
        {
            if (view.OwnerActorNr == actorNumber && view.CompareTag("Player"))
            {
                return view;
            }
        }

        return null;
    }

    private void GrantPickup(GameObject player)
    {
        if (gameObject.layer == ThrowableLayer)
        {
            ThrowableManager throwableManager = player.GetComponent<ThrowableManager>();
            if (throwableManager == null)
            {
                Debug.LogError("Falta ThrowableManager en el player.");
                return;
            }

            throwableManager.ThrowableUp(gameObject);
        }
        else if (gameObject.layer == MedicKitLayer)
        {
            MedicKitManager medicKitManager = player.GetComponent<MedicKitManager>();
            if (medicKitManager == null)
            {
                Debug.LogError("Falta MedicKitManager en el player.");
                return;
            }

            medicKitManager.ThrowableUp(gameObject);
        }
        else if (CompareTag("Armor"))
        {
            PhotonView playerView = player.GetComponent<PhotonView>();
            if (PhotonNetwork.InRoom)
            {
                playerView.RPC(nameof(LifeManager.RPC_GotArmor), RpcTarget.All);
            }
            else
            {
                LifeManager lifeManager = player.GetComponent<LifeManager>();
                if (lifeManager == null)
                {
                    Debug.LogError("Falta LifeManager en el player.");
                    return;
                }

                lifeManager.ApplyArmor();
            }
        }
    }
}
