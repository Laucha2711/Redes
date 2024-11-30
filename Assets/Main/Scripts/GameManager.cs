using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    float TimeToLose;

    [SerializeField]
    private int PointsToWin = 10;
    private int currentPoints;

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
        SceneManager.LoadScene("Menu");
        Debug.Log("ILose");
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
}
