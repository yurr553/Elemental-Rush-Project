using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameType
{
    Moves,
    Time
}


[System.Serializable]
public class EndGameRequirements
{
    public GameType gameType;
    public int counterValue;
}

public class EndGameManager : MonoBehaviour
{
    public EndGameRequirements requirements;
    public GameObject movesLabel, timeLabel;
    public Text counter;
    public int currentCounterValue;
    private float timerSeconds;

    public GameObject youWinPanel, tryAgainPanel;

    private Board board;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        board = FindFirstObjectByType<Board>();
        SetGameType();
        SetupGame();
    }

    void SetGameType()
    {
        if (board.world != null)
        {
            if (board.level < board.world.levels.Length)
            {
                if (board.world.levels[board.level] != null)
                {
                    requirements = board.world.levels[board.level].endGameRequirements;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (requirements.gameType == GameType.Time && currentCounterValue > 0)
        {
            
            if (board != null && board.currentState == GameState.pause) return;

            timerSeconds -= Time.deltaTime;
            if (timerSeconds <= 0f)
            {
                DecreaseCounterValue(); 
                timerSeconds = 1f;
            }
        }
    }

    void SetupGame()
    {
        currentCounterValue = requirements.counterValue;
        if (requirements.gameType == GameType.Moves)
        {
            movesLabel.SetActive(true);
            timeLabel.SetActive(false);
        } else
        {
            movesLabel.SetActive(false);
            timeLabel.SetActive(true);
            timerSeconds = 1;
        }

        counter.text = "" + currentCounterValue;
    }


    public void DecreaseCounterValue()
    {
        if (board.currentState != GameState.pause)
        { 
            currentCounterValue--;
            counter.text = "" + currentCounterValue;
            if (currentCounterValue <= 0)
            {
                LoseGame();
            }
        }
    }

    public void WinGame()
    {
        youWinPanel.SetActive(true);
        board.currentState = GameState.win;
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;
        var fade = FindFirstObjectByType<FadePanelController>(FindObjectsInactive.Include);
        if (fade != null)
        {
            fade.GameOver();
        }
        else
        {
            Debug.LogWarning("FadePanelController not found (likely inactive). Win continues without fade.");
        }
    }

    public void LoseGame()
    {
        tryAgainPanel.SetActive(true);
        board.currentState = GameState.lose;
        Debug.Log("You lose!");
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;
        FadePanelController fade = FindFirstObjectByType<FadePanelController>();
        fade.GameOver();
    }
}
