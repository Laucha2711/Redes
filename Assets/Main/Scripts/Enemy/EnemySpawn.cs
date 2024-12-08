using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField]
    private GameObject enemy;

    private float currentTime;
    private float time = 3;

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient == true)
        {
            time = time - 1 * Time.deltaTime;

            if (time <= 0)
            {
                PhotonNetwork.Instantiate("enemy", transform.position, Quaternion.identity);
                time = 3;
            }
        }
    }
}
