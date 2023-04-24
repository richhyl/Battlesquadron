using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testfadein : MonoBehaviour
{
    public float fadeInTime = 1f;
    public float fadeOutTime = 1f;
    public float[] waitTimes; // Array of wait times
    private CanvasGroup canvasGroup;
    public GameObject[] gameObjects;
    private int currentIndex;

    private void Start()
    {
        currentIndex = 0;
        canvasGroup = gameObjects[currentIndex].GetComponent<CanvasGroup>();
        StartCoroutine(FadeObjects());
    }

    public IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeInTime)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeOutTime)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator FadeObjects()
    {
        while (currentIndex < gameObjects.Length)
        {
            // fade in
            yield return StartCoroutine(FadeIn());

            // wait for a defined amount of time
            float waitTime = waitTimes[currentIndex];
            yield return new WaitForSeconds(waitTime);

            // fade out
            yield return StartCoroutine(FadeOut());

            // move to the next object
            currentIndex++;
            if (currentIndex < gameObjects.Length)
            {
                canvasGroup = gameObjects[currentIndex].GetComponent<CanvasGroup>();
            }
        }
    }
}
