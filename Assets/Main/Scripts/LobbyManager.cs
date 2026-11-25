using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public InputField roomInputField;
    public GameObject lobbyPanel;
    public GameObject roomPanel;
    public Text roomName;
    public Text connectionStatus;

    public EnterRoom roomItemPrefab;
    List<EnterRoom> roomItemsList = new List<EnterRoom>();
    Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
    public Transform contentObject;

    public List<PlayerItem> playerItemsList = new List<PlayerItem>();
    public PlayerItem playerItemPrefab;
    public Transform playerItemParent;

    public GameObject playButton;
    private PhotonView photonView;

    private void SetLobbyStatus(string status)
    {
        connectionStatus.text = "Lobby status: " + status;
    }

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (PhotonNetwork.IsConnectedAndReady)
        {
            SetLobbyStatus("joining lobby...");
            PhotonNetwork.JoinLobby();
        }
        else
        {
            SetLobbyStatus("connecting...");
        }
    }

    public void OnClickCreate()
    {
        if (roomInputField.text.Length >= 1)
        {
            SetLobbyStatus("creating room...");
            PhotonNetwork.CreateRoom(roomInputField.text, new RoomOptions() { MaxPlayers = 2 });
        }
        else
        {
            SetLobbyStatus("enter room name");
        }
    }

    public override void OnJoinedRoom()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomName.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateCachedRoomList(roomList);
        UpdateRoomList();
    }

    public override void OnJoinedLobby()
    {
        SetLobbyStatus("ready to join or create");
        cachedRoomList.Clear();
        UpdateRoomList();
    }

    void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            if (info.RemovedFromList)
            {
                cachedRoomList.Remove(info.Name);
            }
            else
            {
                cachedRoomList[info.Name] = info;
            }
        }
    }

    void UpdateRoomList()
    {
        foreach (EnterRoom item in roomItemsList)
        {
            Destroy(item.gameObject);
        }
        roomItemsList.Clear();

        foreach (KeyValuePair<string, RoomInfo> roomEntry in cachedRoomList)
        {
            RoomInfo room = roomEntry.Value;

            if (!room.IsOpen || (room.MaxPlayers > 0 && room.PlayerCount >= room.MaxPlayers))
            {
                continue;
            }

            EnterRoom newRoom = Instantiate(roomItemPrefab, contentObject);
            newRoom.SetRoomName(room.Name, room.PlayerCount, room.MaxPlayers);
            roomItemsList.Add(newRoom);
        }
    }

    public void JoinRoom(string roomName)
    {
        SetLobbyStatus("joining room...");
        PhotonNetwork.JoinRoom(roomName);
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        SetLobbyStatus("ready to join or create");
    }

    public override void OnConnectedToMaster()
    {
        SetLobbyStatus("joining lobby...");
        PhotonNetwork.JoinLobby();
    }

    void UpdatePlayerList()
    {
        foreach (PlayerItem item in playerItemsList)
        {
            Destroy(item.gameObject);
        }
        playerItemsList.Clear();

        if (PhotonNetwork.CurrentRoom == null)
        {
            return;
        }

        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem newPlayerItem = Instantiate(playerItemPrefab, playerItemParent);
            newPlayerItem.SetPlayerInfo(player.Value);
            playerItemsList.Add(newPlayerItem);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }

    private void Update()
    {
        /*if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            playButton.SetActive(true);
        }else
        {
            playButton.SetActive(false);
        }
        */
    }

    public void OnClickPlayButton()
    {
        if (PhotonNetwork.IsMasterClient && photonView != null)
        {
            photonView.RPC(nameof(RPC_StartGame), RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void RPC_StartGame()
    {
        PhotonNetwork.LoadLevel("Level1");
    }
}
