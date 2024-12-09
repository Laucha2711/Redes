using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class Enemy : MonoBehaviour
{
    private GameObject target;
    private NavMeshAgent agent;
    [SerializeField]
    private GameManager GM;

    private void Start()
    {
        target = GameObject.Find("Player(Clone)");
        agent = GetComponent<NavMeshAgent>();

        if (GM == null)
        {
            GameObject GMtemp = GameObject.Find("GM");
            GM = GMtemp.GetComponent<GameManager>();
        }
    }

    private void Update()
    {
        if (target != null)
        {
            agent.SetDestination(target.transform.position);
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
