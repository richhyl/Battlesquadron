using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testtypewriter : MonoBehaviour
{
    public float delay = 0.1f;                  // Delay between each character
    public Text textComponent;                 // Text component to display the text
    public string[] texts;                     // Array of texts to be typed
    public GameObject objectToHide;            // GameObject to hide after all text has been displayed
    public GameObject objectToShow;            // GameObject to show after all text has been displayed

    private int currentTextIndex = 0;          // Index of the current text being displayed
    private Coroutine typingCoroutine;         // Coroutine for typing effect

    void Start()
    {
        // Start typing the text
        typingCoroutine = StartCoroutine(TypeText());
    }

    void Update()
    {
        // Skip typing effect if space bar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopCoroutine(typingCoroutine);
            textComponent.text = texts[currentTextIndex];
            StartCoroutine(ClearText());
        }
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

        StartCoroutine(ClearText());
    }

    IEnumerator ClearText()
    {
        yield return new WaitForSeconds(delay);

        textComponent.text = "";

        currentTextIndex++;
        if (currentTextIndex >= texts.Length)
        {
            // All texts have been displayed, hide objectToHide and show objectToShow
            if (objectToHide != null)
            {
                objectToHide.SetActive(false);
            }

            if (objectToShow != null)
            {
                objectToShow.SetActive(true);
            }

            // Stop typing
            StopCoroutine(typingCoroutine);
            yield break;
        }

        // Continue typing
        typingCoroutine = StartCoroutine(TypeText());
    }
}
