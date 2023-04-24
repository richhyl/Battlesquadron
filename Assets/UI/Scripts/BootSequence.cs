using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.SceneManagement;

public class BootSequence : MonoBehaviour
{
    public bool OnStartSwitchToBoot = true;
    //public GameObject GameTitle;
    //public GameObject IntroSlides;
    public GameObject MainMenu;
    public GameObject InGameUI;


    public int BootTextWaitTime = 5;

    public void Start()
    {
        if (OnStartSwitchToBoot)
        {
            Cursor.visible = false;
            //GameTitle.SetActive(false);
            //IntroSlides.SetActive(false);
            MainMenu.SetActive(false);
            InGameUI.SetActive(false);
            //StartCoroutine(DisplayScene());
        }
        //DontDestroyOnLoad(UI);
    }
    //IEnumerator DisplayScene()
    //{
    //    GameTitle.SetActive(true);
    //    CanvasGroup canvasGroup = GameTitle.GetComponent<CanvasGroup>(); // Get the CanvasGroup component from the GameTitle object
    //    canvasGroup.alpha = 1f; // Set the initial alpha value to 1 (fully opaque)
    //    while (canvasGroup.alpha > 0) // Continue until the alpha value reaches 0 (fully transparent)
    //    {
    //        canvasGroup.alpha -= Time.deltaTime / BootTextWaitTime; // Decrease the alpha value over time based on BootTextWaitTime
    //        yield return null; // Wait for a frame
    //    }
    //    // GameTitle.SetActive(false); // Set GameTitle to inactive after the fade effect
    //    //IntroSlides.SetActive(true);
    //    Cursor.visible = true;
    //}

}