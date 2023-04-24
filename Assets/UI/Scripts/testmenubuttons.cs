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
    public string sceneToLoad;
    public GameObject fadeObject;
    public float fadeDuration = 1.0f;

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
        for (int i = 0; i < customKeys.Length; i++)
        {
            if (Input.GetKeyDown(customKeys[i]) && !fading)
            {
                if (customKeys[i] == KeyCode.F5)
                {
                    StartCoroutine(LoadSceneWithFade(sceneToLoad)); //TODO need to load the proper scene a string is no good here. Build index would be best.
                }
                else
                {
                    ToggleSprite(images[i], oldSprites[i], newSprites[i]);
                    if (customKeys[i] == KeyCode.F5)
                    {
                        AudioListener.volume = musicVolume;
                    }
                }
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

    IEnumerator LoadSceneWithFade(string sceneName)
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

        SceneManager.LoadScene(sceneName);
    }
}
