using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public GameObject target;
    private NavMeshAgent agent;
    [SerializeField]
    private GameManager GM;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
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
        FindObjectOfType<GameManager>().SpawnItem(transform.position);
        GM.PointsUp();
        Destroy(gameObject);
    }
}
