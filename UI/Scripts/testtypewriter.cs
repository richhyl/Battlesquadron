using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testtypewriter : MonoBehaviour
{
    public float delay = 0.1f;                  // Delay between each character
    public Text textComponent;                 // Text component to display the text
    public List<Image> imageComponents;        // List of Image components to apply fade effects
    public GameObject targetGameObject;        // GameObject to make visible
    public string[] texts;                     // Array of texts to be typed
    public float fadeDuration = 1.0f;           // Duration of the fade effect
    public Color fadeColor;                    // Color of the fade effect

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
            currentTextIndex = 0;
            if (targetGameObject != null)
            {
                targetGameObject.SetActive(true);
            }
        }

        foreach (Image imageComponent in imageComponents)
        {
            StartCoroutine(FadeImage(imageComponent));
        }
    }

    IEnumerator FadeImage(Image imageComponent)
    {
        float elapsedTime = 0;
        Color originalColor = imageComponent.color;
        Color targetColor = fadeColor;
        while (elapsedTime < fadeDuration)
        {
            float t = elapsedTime / fadeDuration;
            imageComponent.color = Color.Lerp(originalColor, targetColor, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        imageComponent.color = targetColor;
        typingCoroutine = StartCoroutine(TypeText());
    }
}
