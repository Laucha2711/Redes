using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnterRoom : MonoBehaviour
{
    public Text roomName;
    private string currentRoomName;
    LobbyManager manager;

    private void Start()
    {
        manager = FindObjectOfType<LobbyManager>();
    }

    public void SetRoomName(string _roomName, int playerCount, int maxPlayers)
    {
        currentRoomName = _roomName;
        roomName.text = _roomName + " (" + playerCount + "/" + maxPlayers + ")";
    }

    public void OnClickItem()
    {
        manager.JoinRoom(currentRoomName);
    }
}
