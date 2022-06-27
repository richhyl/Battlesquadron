using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public GameObject Background;
    public GameObject RetroPlayPanel;
    public GameObject RetroOptionsPanel;
    public GameObject RetroCheatsPanel;
    public GameObject RetroIntroPanel;
    public GameObject ModernPlayPanel;
    public GameObject ModernOptionsPanel;
    public GameObject ModernCheatsPanel;
    public GameObject ModernIntroPanel;
    CanvasGroup thisCanvas;
    public CanvasGroup InGameHUD;

    public void RetroPlayGame()
    {
        RetroPlayPanel.SetActive(false);
        Background.SetActive(false);
        RetroIntroPanel.SetActive(true);
    }

    public void RetroDisplayOptions()
    {
        RetroPlayPanel.SetActive(false);
        RetroOptionsPanel.SetActive(true);
    }

    public void RetroDisplayCheats()
    {
        RetroPlayPanel.SetActive(false);
        RetroCheatsPanel.SetActive(true);
    }

    public void RetroDisplayPlay()
    {
        RetroOptionsPanel.SetActive(false);
        RetroCheatsPanel.SetActive(false);
        RetroPlayPanel.SetActive(true);
    }

    public void ModernPlayGame()
    {
        ModernPlayPanel.SetActive(false);
        Background.SetActive(false);
        ModernIntroPanel.SetActive(true);
    }

    public void ModernDisplayOptions()
    {
        ModernPlayPanel.SetActive(false);
        ModernOptionsPanel.SetActive(true);
    }

    public void ModernDisplayCheats()
    {
        ModernPlayPanel.SetActive(false);
        ModernCheatsPanel.SetActive(true);
    }

    public void ModernDisplayPlay()
    {
        ModernOptionsPanel.SetActive(false);
        ModernCheatsPanel.SetActive(false);
        ModernPlayPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT GAME");
        Application.Quit();
    }
    private void Start()
    {
        thisCanvas = gameObject.GetComponent<CanvasGroup>();
    }
}
