using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLvl : MonoBehaviour
{
    [SerializeField]
    private GameManager GM;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GM.WinGame();
        }
    }
}
