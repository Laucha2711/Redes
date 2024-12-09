using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Destroy : MonoBehaviour
{
    private void Awake()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
