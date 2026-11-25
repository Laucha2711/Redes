using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPunCallbacks
{
    private const string CurrentPointsProperty = "currentPoints";

    [SerializeField]
    float TimeToLose;

    [SerializeField]
    private int PointsToWin = 10;
    private int currentPoints;
    private bool returnToLobbyAfterDeath;

    public GameObject[] spawnItems;
    public float[] spawnItemsChance;
    public Text disconnectMessage;
    public Text masterClientMessage;
    private Coroutine hideMessagesCoroutine;

    private void Awake()
    {
        disconnectMessage.gameObject.SetActive(false);
        masterClientMessage.gameObject.SetActive(false);

        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient &&
                !PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(CurrentPointsProperty))
            {
                SetCurrentPoints(0);
            }

            currentPoints = GetCurrentPoints();
        }
        else
        {
            currentPoints = 0;
        }
    }

    private void FixedUpdate()
    {
        TimeToLose = TimeToLose - 1 * Time.deltaTime;

        if (TimeToLose <= 0)
        {
            LoseGame();
        }
    }
    public void LoseGame()
    {
        SceneManager.LoadScene("ConnectLobby");
        Debug.Log("ILose");
    }

    public void LeaveRoomAfterPlayerDeath()
    {
        if (PhotonNetwork.InRoom)
        {
            returnToLobbyAfterDeath = true;
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            SceneManager.LoadScene("Lobby");
        }
    }

    public override void OnLeftRoom()
    {
        if (returnToLobbyAfterDeath)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene("Lobby");
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("player disconnected: " + otherPlayer.NickName);
        ShowMessage(disconnectMessage, "player disconnected: " + otherPlayer.NickName);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        currentPoints = GetCurrentPoints();
        Debug.Log("master changed: " + newMasterClient.NickName);
        ShowMessage(masterClientMessage, "master changed: " + newMasterClient.NickName);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(CurrentPointsProperty))
        {
            currentPoints = (int)propertiesThatChanged[CurrentPointsProperty];
        }
    }

    private int GetCurrentPoints()
    {
        if (PhotonNetwork.InRoom &&
            PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(CurrentPointsProperty))
        {
            return (int)PhotonNetwork.CurrentRoom.CustomProperties[CurrentPointsProperty];
        }

        return currentPoints;
    }

    private bool SetCurrentPoints(int points)
    {
        if (PhotonNetwork.CurrentRoom.SetCustomProperties(
            new Hashtable { { CurrentPointsProperty, points } }))
        {
            return true;
        }

        Debug.LogError("could not sync game points");
        return false;
    }

    private void ShowMessage(Text message, string text)
    {
        message.text = text;
        message.transform.root.gameObject.SetActive(true);
        message.gameObject.SetActive(true);

        if (hideMessagesCoroutine != null)
        {
            StopCoroutine(hideMessagesCoroutine);
        }

        hideMessagesCoroutine = StartCoroutine(HideMessages());
    }

    private IEnumerator HideMessages()
    {
        yield return new WaitForSeconds(3f);
        disconnectMessage.gameObject.SetActive(false);
        masterClientMessage.gameObject.SetActive(false);
        disconnectMessage.transform.root.gameObject.SetActive(false);
        hideMessagesCoroutine = null;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("ConnectLobby");
    }

    public void WinGame()
    {
        SceneManager.LoadScene("Menu");
        Debug.Log("IWIn");
    }
    public void NextLvl()
    {
        SceneManager.LoadScene("Level2");
        Cursor.lockState = CursorLockMode.None;
        Debug.Log("Level2");
    }

    public void PointsUp()
    {
        if (!PhotonNetwork.InRoom || !PhotonNetwork.IsMasterClient)
        {
            return;
        }

        currentPoints = GetCurrentPoints();
        currentPoints++;
        if (!SetCurrentPoints(currentPoints))
        {
            currentPoints--;
            return;
        }

        if (currentPoints >= PointsToWin)
        {
            WinGame();
        }
    }

    public void SpawnItem(Vector3 enemyPos)
    {
        float x = Random.Range(0f, 100f);
        float sum = 0f;

        for (int i = 0; i <spawnItemsChance.Length; i++)
        {
            sum += spawnItemsChance[i];

            if (x < sum)
            {
                PhotonNetwork.Instantiate(spawnItems[i].name.ToString(), enemyPos, spawnItems[i].transform.rotation);
                break;
            }
        }
    }
}
