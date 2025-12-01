using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject wordlistPanel;
    public GameObject pausePanel;
    private Board board;
    public bool paused = false;
    public Image soundButton;
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;
    private SoundManager soundManager;

    [SerializeField]
    private GameObject[] alsoPauseWhenActive;

    void Start()
    {
        soundManager = FindFirstObjectByType<SoundManager>();
        board = GameObject.FindWithTag("Board").GetComponent<Board>();

        if (pausePanel) pausePanel.SetActive(false);
        if (wordlistPanel) wordlistPanel.SetActive(false);

        if (PlayerPrefs.HasKey("Sound"))
        {
            soundButton.sprite = PlayerPrefs.GetInt("Sound") == 0 ? musicOffSprite : musicOnSprite;
        }
        else
        {
            soundButton.sprite = musicOnSprite;
        }
    }

    void Update()
    {
        bool anyExtraActive = false;
        if (alsoPauseWhenActive != null)
        {
            foreach (var go in alsoPauseWhenActive)
            {
                if (go != null && go.activeInHierarchy)
                {
                    anyExtraActive = true;
                    break;
                }
            }
        }

        bool shouldPause =
            paused ||
            (wordlistPanel != null && wordlistPanel.activeInHierarchy) ||
            anyExtraActive;

        bool anyWordlistActive = (wordlistPanel != null && wordlistPanel.activeInHierarchy) || anyExtraActive;

        if (anyWordlistActive)
        {
            if (pausePanel && pausePanel.activeInHierarchy)
                pausePanel.SetActive(false);
        }
        else
        {
            if (pausePanel)
            {
                if (paused && !pausePanel.activeInHierarchy)
                    pausePanel.SetActive(true);
                else if (!paused && pausePanel.activeInHierarchy)
                    pausePanel.SetActive(false);
            }
        }

        if (board)
        {
            if (shouldPause && board.currentState != GameState.pause)
                board.currentState = GameState.pause;
        }
    }

    public void PauseGame()
    {
        paused = !paused;

        if (!board) return;

        if (paused)
        {
            board.currentState = GameState.pause;
        }
        else
        {
            board.currentState = GameState.move;   
        }
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("Main");
    }

    public void SoundButton()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                soundButton.sprite = musicOnSprite;
                PlayerPrefs.SetInt("Sound", 1);
                soundManager.adjustVolume();
            }
            else
            {
                soundButton.sprite = musicOffSprite;
                PlayerPrefs.SetInt("Sound", 0);
                soundManager.adjustVolume();
            }
        }
        else
        {
            soundButton.sprite = musicOffSprite;
            PlayerPrefs.SetInt("Sound", 1);
            soundManager.adjustVolume();
        }
    }

    public void WordlistButton()
    {
        if (wordlistPanel) wordlistPanel.SetActive(true);
        if (pausePanel) pausePanel.SetActive(false);
    }

    public void CloseWordlist()
    {
        if (wordlistPanel) wordlistPanel.SetActive(false);

        paused = false;
        if (pausePanel) pausePanel.SetActive(false);

        if (board && board.currentState == GameState.pause)
            board.currentState = GameState.move;
    }
}
