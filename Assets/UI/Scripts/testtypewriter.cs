using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class testtypewriter : MonoBehaviour
{
    public float delay = 0.1f;                  // Delay between each character
    public float[] pauseDelays;                 // Array of pause delays, one for each text
    public TMP_Text textComponent;              // Text component to display the text
    public string[] texts;                     // Array of texts to be typed
    public GameObject hideGameObject;           // Game object to hide
    public GameObject showGameObject;           // Game object to show

    private int currentTextIndex = 0;          // Index of the current text being displayed
    private Coroutine typingCoroutine;         // Coroutine for typing effect

    void Start()
    {
        // Start typing the text
        typingCoroutine = StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        string currentText = texts[currentTextIndex];
        textComponent.text = "";

        for (int i = 0; i < currentText.Length; i++)
        {
            textComponent.text += currentText[i];
            yield return new WaitForSeconds(delay);
        }

        StartCoroutine(PauseAndClearText());
    }

    IEnumerator PauseAndClearText()
    {
        float pauseDelay = pauseDelays[currentTextIndex];
        yield return new WaitForSeconds(pauseDelay);

        textComponent.text = "";

        currentTextIndex++;
        if (currentTextIndex >= texts.Length)
        {
            hideGameObject.SetActive(false);
            showGameObject.SetActive(true);
            yield break;
        }

        typingCoroutine = StartCoroutine(TypeText());
    }
}
