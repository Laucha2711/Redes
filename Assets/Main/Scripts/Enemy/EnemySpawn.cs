using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField]
    private GameObject enemy;

    private float currentTime;
    private float time = 3;

    private void Update()
    {
        time = time - 1 * Time.deltaTime;

        if (time <= 0)
        {
            Instantiate(enemy, transform.position, Quaternion.identity);
            time = 3;
        }
    }
}
