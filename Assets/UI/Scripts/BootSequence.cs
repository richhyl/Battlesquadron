using System.Collections;
using UnityEngine;


public class BootSequence : MonoBehaviour
{
    public bool OnStartSwitchToBoot = true;
    public GameObject IntroSequence;
    public GameObject MainMenu;
    public GameObject InGameHUD;

    public int BootTextWaitTime = 5;

    public void Start()
    {
        if (OnStartSwitchToBoot)
        {
            Cursor.visible = false;
            IntroSequence.SetActive(true);
            MainMenu.SetActive(false);
            InGameHUD.SetActive(false);
        }
        //DontDestroyOnLoad(UI);
    }
}