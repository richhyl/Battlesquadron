using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class testmenubuttons : MonoBehaviour
{
    public Image[] images;
    public Sprite[] newSprites;
    public KeyCode[] customKeys;
    public float musicVolume = 0;
    public int sceneIndexOffset = 1;
    public GameObject fadeObject;
    public float fadeDuration = 1.0f;
    public GameObject hideObject;
    public GameObject showObject;

    private Sprite[] oldSprites;
    private bool fading = false;

    void Start()
    {
        oldSprites = new Sprite[images.Length];
        for (int i = 0; i < images.Length; i++)
        {
            if (images[i] != null)
            {
                oldSprites[i] = images[i].sprite;
            }
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && !fading)
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + sceneIndexOffset;
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                if (hideObject != null)
                {
                    hideObject.SetActive(false);
                }
                if (showObject != null)
                {
                    showObject.SetActive(true);
                }
                StartCoroutine(LoadSceneWithFade(nextSceneIndex));
            }
        }

        for (int i = 0; i < customKeys.Length; i++)
        {
            if (Input.GetKeyDown(customKeys[i]) && !fading && customKeys[i] != KeyCode.F5)
            {
                ToggleSprite(images[i], oldSprites[i], newSprites[i]);
            }
        }
    }

    void ToggleSprite(Image image, Sprite oldSprite, Sprite newSprite)
    {
        if (image != null)
        {
            if (image.sprite == oldSprite)
            {
                image.sprite = newSprite;
            }
            else
            {
                image.sprite = oldSprite;
            }
        }
    }

    IEnumerator LoadSceneWithFade(int sceneIndex)
    {
        fading = true;
        if (fadeObject != null)
        {
            Image fadeImage = fadeObject.GetComponent<Image>();
            if (fadeImage != null)
            {
                Color fadeColor = fadeImage.color;
                while (fadeColor.a < 1.0f)
                {
                    fadeColor.a += Time.deltaTime / fadeDuration;
                    fadeImage.color = fadeColor;
                    yield return null;
                }
            }
        }

        SceneManager.LoadScene(sceneIndex);
    }
}
