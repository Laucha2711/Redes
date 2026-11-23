using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class Enemy : MonoBehaviour
{
    private GameObject target;
    private NavMeshAgent agent;
    private PhotonView PV;
    [SerializeField]
    private GameManager GM;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        PV = GetComponent<PhotonView>();

        if (GM == null)
        {
            GameObject GMtemp = GameObject.Find("GM");
            if (GMtemp != null)
            {
                GM = GMtemp.GetComponent<GameManager>();
            }
        }

        if (PV.IsMine)
        {
            FindNearestPlayer();
        }
    }

    private void Update()
    {
        if (PV.IsMine)
        {
            if (target == null)
            {
                FindNearestPlayer();
            }

            if (target != null)
            {
                agent.SetDestination(target.transform.position);
            }
        }
    }

    private void FindNearestPlayer()
    {
        float nearestDistance = Mathf.Infinity;
        target = null;

        foreach (PhotonView view in FindObjectsOfType<PhotonView>())
        {
            if (view.gameObject.CompareTag("Player"))
            {
                float distance = Vector3.Distance(transform.position, view.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    target = view.gameObject;
                }
            }
        }
    }


    public void Dead()
    {
        if (PhotonNetwork.IsMasterClient == true)
        {
            FindObjectOfType<GameManager>().SpawnItem(transform.position);
            GM.PointsUp();
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
