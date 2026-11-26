using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform spawnPoint;

    private void Start()
    {
        int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        int column = playerIndex % 2;
        int row = playerIndex / 2;
        Vector3 position = spawnPoint.position + new Vector3(column * 2f, 0f, row * 2f);

        PhotonNetwork.Instantiate(playerPrefab.name, position, spawnPoint.rotation);

        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.ReportPlayerSpawned();
        }
        else
        {
            Debug.LogError("game manager not found");
        }
    }
}
