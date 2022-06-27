//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class BootSequence : MonoBehaviour
//{
//    public bool OnStartSwitchToBoot = true;

//    public GameObject SplashPanel;
//    public GameObject IntroPanel;
//    public GameObject IntroSlide1;
//    public GameObject IntroSlide2;
//    public GameObject IntroSlide3;
//    public GameObject IntroSlide4;
//    public GameObject IntroSlide5;
//    public GameObject IntroSlide6;
//    public GameObject IntroSlide7;
//    public GameObject RetroMenu;

//    public int BootTextWaitTime = 5;

//    public void Start()
//    {
//        if (OnStartSwitchToBoot)
//        {
//            Cursor.visible = false;
//            SplashPanel.SetActive(false);
//            IntroPanel.SetActive(false);
//            IntroSlide1.SetActive(false);
//            IntroSlide2.SetActive(false);
//            IntroSlide3.SetActive(false);
//            IntroSlide4.SetActive(false);
//            IntroSlide5.SetActive(false);
//            IntroSlide6.SetActive(false);
//            IntroSlide7.SetActive(false);
//            RetroMenu.SetActive(false);

//            StartCoroutine(DisplayScene());
//        }
//    }

//    IEnumerator DisplayScene()
//    {
//        SplashPanel.SetActive(true);
//        // yield return new WaitForSeconds(BootTextWaitTime);
//        // SplashScreen.SetActive(false);
//        // MainMenu.SetActive(true);
//        // Cursor.visible = true;
//        //IntroPanel.SetActive(true);
//        //IntroSlide1.SetActive(true);
//        //yield return new WaitForSeconds(25);
//        //IntroSlide1.SetActive(false);
//        //IntroSlide2.SetActive(true);
//        //yield return new WaitForSeconds(25);
//        //IntroSlide2.SetActive(false);
//        //IntroSlide3.SetActive(true);
//        //yield return new WaitForSeconds(13);
//        //IntroSlide3.SetActive(false);
//        //IntroSlide4.SetActive(true);
//        //yield return new WaitForSeconds(30);
//        //Boot.SetActive(false);
//        //MainMenu.SetActive(true);
//    }

//}