using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    float TimeToLose;

    [SerializeField]
    private int PointsToWin = 10;
    private int currentPoints;
    private bool returnToLobbyAfterDeath;

    public GameObject[] spawnItems;
    public float[] spawnItemsChance;
    public Text disconnectMessage;

    private void Awake()
    {
        currentPoints = 0;
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
        disconnectMessage.text = "player disconnected: " + otherPlayer.NickName;
        disconnectMessage.gameObject.SetActive(true);
        StartCoroutine(HideDisconnectMessage());
    }

    private IEnumerator HideDisconnectMessage()
    {
        yield return new WaitForSeconds(3f);
        disconnectMessage.gameObject.SetActive(false);
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
        currentPoints++;
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
