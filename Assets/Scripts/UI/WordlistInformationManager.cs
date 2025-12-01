using UnityEngine;
using UnityEngine.UI;


public class WordlistInformationManager : MonoBehaviour
{
    public GameObject wordlistPanel;
    public GameObject wordlistinfobackground;
    public GameObject waterPanelInfo;
    public GameObject gasPanelInfo;
    public GameObject acidPanelAcid;

    private Board board;
    private PauseManager pauseManager;

    void Start()
    {
        board = FindFirstObjectByType<Board>();
        pauseManager = FindAnyObjectByType<PauseManager>();

    }

    
    void Update()
    {
        
    }

    public void ReturnToWordlist()
    {
        wordlistPanel.SetActive(true);
        wordlistinfobackground.SetActive(false);
        waterPanelInfo.SetActive(false);
        gasPanelInfo.SetActive(false);
        acidPanelAcid.SetActive(false);
    }

    public void WaterInfo()
    {
        wordlistinfobackground.SetActive(true);
        waterPanelInfo.SetActive(true);
        pauseManager.wordlistPanel.SetActive(false);
        if (pauseManager != null) pauseManager.wordlistPanel.SetActive(false);
    }

    public void GasInfo()
    {
        wordlistinfobackground.SetActive(true);
        gasPanelInfo.SetActive(true);
        pauseManager.wordlistPanel.SetActive(false);
        if (pauseManager != null) pauseManager.wordlistPanel.SetActive(false);
    }

    public void AcidInfo()
    {
        wordlistinfobackground.SetActive(true);
        acidPanelAcid.SetActive(true);
        pauseManager.wordlistPanel.SetActive(false);
        if (pauseManager != null) pauseManager.wordlistPanel.SetActive(false);
    }
}
