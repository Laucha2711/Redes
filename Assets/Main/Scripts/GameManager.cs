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
    public const string CurrentPointsProperty = "currentPoints";
    public const string GameStartRoundProperty = "gameStartRound";
    public const string GameStartTimestampProperty = "gameStartTimestamp";
    private const string ReadyRoundProperty = "readyRound";
    private const int StartDelayMilliseconds = 3000;

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
    public Text gameStartCountdown;
    private Coroutine hideMessagesCoroutine;
    private bool localPlayerSpawned;
    private int reportedReadyRound = int.MinValue;
    private bool hasGameStartTime;
    private int gameStartTimestamp;

    public static bool GameStarted { get; private set; }

    private void Awake()
    {
        GameStarted = false;
        disconnectMessage.gameObject.SetActive(false);
        masterClientMessage.gameObject.SetActive(false);
        gameStartCountdown.gameObject.SetActive(true);
        gameStartCountdown.text = "waiting for players...";

        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient &&
                !PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(CurrentPointsProperty))
            {
                SetCurrentPoints(0);
            }

            currentPoints = GetCurrentPoints();
            object startTimestamp;
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(
                GameStartTimestampProperty, out startTimestamp) && startTimestamp is int)
            {
                gameStartTimestamp = (int)startTimestamp;
                hasGameStartTime = true;
            }
        }
        else
        {
            currentPoints = 0;
        }
    }

    private void FixedUpdate()
    {
        if (!GameStarted)
        {
            return;
        }

        TimeToLose = TimeToLose - 1 * Time.deltaTime;

        if (TimeToLose <= 0)
        {
            LoseGame();
        }
    }

    private void Update()
    {
        if (!PhotonNetwork.InRoom || GameStarted)
        {
            return;
        }

        if (!hasGameStartTime)
        {
            gameStartCountdown.text = "waiting for players...";
            return;
        }

        int timeRemaining = unchecked(gameStartTimestamp - PhotonNetwork.ServerTimestamp);
        if (timeRemaining > 0)
        {
            gameStartCountdown.text = "starting in: " + Mathf.CeilToInt(timeRemaining / 1000f);
            return;
        }

        GameStarted = true;
        gameStartCountdown.gameObject.SetActive(false);
    }

    public void ReportPlayerSpawned()
    {
        localPlayerSpawned = true;
        TryReportPlayerReady();
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
        TryStartGame();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        currentPoints = GetCurrentPoints();
        Debug.Log("master changed: " + newMasterClient.NickName);
        ShowMessage(masterClientMessage, "master changed: " + newMasterClient.NickName);
        TryStartGame();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey(ReadyRoundProperty))
        {
            TryStartGame();
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(CurrentPointsProperty))
        {
            currentPoints = (int)propertiesThatChanged[CurrentPointsProperty];
        }

        if (propertiesThatChanged.ContainsKey(GameStartRoundProperty))
        {
            TryReportPlayerReady();
            TryStartGame();
        }

        if (propertiesThatChanged.ContainsKey(GameStartTimestampProperty))
        {
            object startTimestamp = propertiesThatChanged[GameStartTimestampProperty];
            if (startTimestamp is int)
            {
                gameStartTimestamp = (int)startTimestamp;
                hasGameStartTime = true;
            }
        }
    }

    private void TryReportPlayerReady()
    {
        if (!localPlayerSpawned || !PhotonNetwork.InRoom ||
            !PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(GameStartRoundProperty))
        {
            return;
        }

        int round = (int)PhotonNetwork.CurrentRoom.CustomProperties[GameStartRoundProperty];
        if (round == reportedReadyRound)
        {
            return;
        }

        if (PhotonNetwork.LocalPlayer.SetCustomProperties(
            new Hashtable { { ReadyRoundProperty, round } }))
        {
            reportedReadyRound = round;
            TryStartGame();
        }
        else
        {
            Debug.LogError("could not send player ready state");
        }
    }

    private void TryStartGame()
    {
        if (!PhotonNetwork.InRoom || !PhotonNetwork.IsMasterClient ||
            !PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(GameStartRoundProperty) ||
            PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(GameStartTimestampProperty))
        {
            return;
        }

        int round = (int)PhotonNetwork.CurrentRoom.CustomProperties[GameStartRoundProperty];
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            object readyRound;
            if (!player.CustomProperties.TryGetValue(ReadyRoundProperty, out readyRound) ||
                !(readyRound is int) || (int)readyRound != round)
            {
                return;
            }
        }

        int startTimestamp = unchecked(PhotonNetwork.ServerTimestamp + StartDelayMilliseconds);
        if (!PhotonNetwork.CurrentRoom.SetCustomProperties(
            new Hashtable { { GameStartTimestampProperty, startTimestamp } }))
        {
            Debug.LogError("could not synchronize game start");
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
