using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class testboot : MonoBehaviour
{
    public bool OnStartSwitchToBoot = true;
    public GameObject GameTitle;
    public GameObject IntroSlides;
    public Image GameTitleImage; // Reference to the Image component of the GameTitle GameObject
    public float FadeOutTime = 1.0f; // Time duration for the fade out effect

    public int BootTextWaitTime = 5;

    public void Start()
    {
        if (OnStartSwitchToBoot)
        {
            Cursor.visible = false;
            GameTitle.SetActive(false);
            IntroSlides.SetActive(false);
            StartCoroutine(DisplayScene());
        }
    }

    IEnumerator DisplayScene()
    {
        GameTitle.SetActive(true);
        yield return new WaitForSeconds(BootTextWaitTime);
        StartCoroutine(FadeOutGameTitle());
        IntroSlides.SetActive(true);
        Cursor.visible = true;
    }

    IEnumerator FadeOutGameTitle()
    {
        float timer = 0.0f;
        Color startColor = GameTitleImage.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0.0f); // Fade out to transparent

        while (timer < FadeOutTime)
        {
            GameTitleImage.color = Color.Lerp(startColor, endColor, timer / FadeOutTime);
            timer += Time.deltaTime;
            yield return null;
        }

        GameTitle.SetActive(false);
    }
}
