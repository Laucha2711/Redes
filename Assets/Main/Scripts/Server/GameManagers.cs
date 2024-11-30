using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class GameManagers : MonoBehaviourPunCallbacks
{
    public TMP_Text textIndicator;

    private void Start()
    {
        if (!PhotonNetwork.IsConnected);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnected()
    {
        base.OnConnected();
        Debug.Log("conectados a photon");
        textIndicator.text = "conectados correctamente";
    }

}
