using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class GameManagers : MonoBehaviourPunCallbacks
{
    public TMP_Text textIndicator;
    public GameObject btnConect;


    private void Start()
    {
        btnConect.SetActive(false);
    }


    public void ConectPhoton()
    {
        if (!PhotonNetwork.IsConnected)
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreatePlayer(string namePlayer)
    {
        PhotonNetwork.NickName = namePlayer;
        PlayerPrefs.SetString("PLayerName", namePlayer);
    }


    // conexion de internet
    public override void OnConnected()
    {
        base.OnConnected();
        Debug.Log("conectados a photon");
        textIndicator.text = "conectados correctamente";
    }

    //verificamos la conexion al servidor de photon
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        textIndicator.text = "Bienvenido " + PhotonNetwork.NickName;
        btnConect.SetActive(true);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
    }

    public void CreateRoom()
    {
        string user = PhotonNetwork.NickName;
        string nameRoom = "sala 1";

        RoomOptions optionRoom = new RoomOptions();
        optionRoom.IsVisible = true;
        optionRoom.MaxPlayers = 2;
        optionRoom.PublishUserId = true;

        PhotonNetwork.JoinOrCreateRoom(nameRoom, optionRoom,TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("estamos conectados a la sala " +  PhotonNetwork.CurrentRoom.Name + " bienvenido " + PhotonNetwork.NickName);
         

        SceneManager.LoadScene("Menu"); 
    }

}
