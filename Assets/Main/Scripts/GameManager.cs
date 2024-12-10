using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    float TimeToLose;

    [SerializeField]
    private int PointsToWin = 10;
    private int currentPoints;

    public GameObject[] spawnItems;
    public float[] spawnItemsChance;

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

    public void WinGame()
    {
        SceneManager.LoadScene("ConnectLobby");
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
