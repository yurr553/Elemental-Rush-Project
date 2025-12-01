using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BackToMain : MonoBehaviour
{
    
    private GameData gameData;
    private Board board;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameData = FindFirstObjectByType<GameData>();
        board = FindFirstObjectByType<Board>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WinOK()
    {
        if (gameData != null)
        {
            gameData.saveData.isActive[board.level + 1] = true;
            gameData.Save();
        }
            SceneManager.LoadScene("Main");
        
    }

    public void LoseOk()
    {

        SceneManager.LoadScene("Main");
    }
}
