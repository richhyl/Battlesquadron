using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenButtons : MonoBehaviour
{

    public GameObject SplashScreen;
    public GameObject RetroMenu;
    public GameObject ModernMenu;
    CanvasGroup thisCanvas;
    //public CanvasGroup InGameHUD;

    //public void PlayRetro()
    //{
    //    SplashScreen.SetActive(false);
    //    yield return new WaitForSeconds(13);
    //    RetroMenu.SetActive(true);
    //}
    public void PlayModern()
    {
        SplashScreen.SetActive(false);
        //Background.SetActive(false);
        ModernMenu.SetActive(true);
    }
    //public void ModernMenu()
    //{
    //    ModernMenu.SetActive(false);
    //    ModernMenu.SetActive(true);
    //}

    //public void RetroDisplayCheats()
    //{
    //    RetroPlayPanel.SetActive(false);
    //    RetroCheatsPanel.SetActive(true);
    //}

    //public void RetroDisplayPlay()
    //{
    //    RetroOptionsPanel.SetActive(false);
    //    RetroCheatsPanel.SetActive(false);
    //    RetroPlayPanel.SetActive(true);
    //}

    //public void ModernPlayGame()
    //{
    //    ModernPlayPanel.SetActive(false);
    //    Background.SetActive(false);
    //    ModernIntroPanel.SetActive(true);
    //}

    //public void ModernDisplayOptions()
    //{
    //    ModernPlayPanel.SetActive(false);
    //    ModernOptionsPanel.SetActive(true);
    //}

    //public void ModernDisplayCheats()
    //{
    //    ModernPlayPanel.SetActive(false);
    //    ModernCheatsPanel.SetActive(true);
    //}

    //public void ModernDisplayPlay()
    //{
    //    ModernOptionsPanel.SetActive(false);
    //    ModernCheatsPanel.SetActive(false);
    //    ModernPlayPanel.SetActive(true);
    //}

    
    //private void Start()
    //{
    //    thisCanvas = gameObject.GetComponent<CanvasGroup>();
    //}
}
