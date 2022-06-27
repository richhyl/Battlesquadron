using System.Collections;
//using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 
    This is pretty getto code i don't know why i went down this confusing path :D. This is very fragile and requires you 
    keep the same menu order of Image as the first component and Text as the second. If you change the order it will give 
    you a compile error.  Anyway Hope it helps. 
 
 */

namespace UI
{
    public class IntroSlides : MonoBehaviour
    {
        public float[] delayTime;
        public CanvasGroup _inGameHUD;
        public float fadeSpeed = 0.1f;

        private int _currentMenuItem;
        private Transform _currentChild;
        private CanvasGroup _thisCanvas;

        void Start()
        {
            _currentChild = transform.GetChild(_currentMenuItem);
            StartCoroutine(WaitAndPrint());
        }

        void Update()
        {
            if (!Input.GetKeyDown("space")) return;
            StopAllCoroutines();

            StartCoroutine(FadeOutObject());

        }

        private IEnumerator FadeOutObject()
        {
            // While the Image alpha of our gameobject is 0 run the below code. 
            while (_currentChild.GetChild(0).GetComponent<Image>().color.a > 0)
            {
                // Get the current colour of the image and the text mesh pro gameobject. 
                Color currentImageColour = _currentChild.GetChild(0).GetComponent<Image>().color;
                Color currentTextColor = _currentChild.GetChild(1).GetComponent<Text>().color;

                // Create a new alpha value based on the current aplha minus the fade speed. 
                float fadeAmount = _currentChild.GetChild(0).GetComponent<Image>().color.a - (fadeSpeed * Time.deltaTime);

                // As we can't directly access the image color we need to create new colours based on the original value and our new fade amount for the Alpha
                Color currentImageFade = new Color(currentImageColour.r, currentImageColour.g, currentImageColour.b, fadeAmount);
                Color currentTextFade = new Color(currentTextColor.r, currentTextColor.g, currentTextColor.b, fadeAmount);

                // Now we assign our new colour values to the component. 
                _currentChild.GetChild(0).GetComponent<Image>().color = currentImageFade;
                _currentChild.GetChild(1).GetComponent<Text>().color = currentTextFade;
                yield return null;
            }

            _currentMenuItem += 1;
            EnableNextScreen();
        }

        private IEnumerator FadeInObject(int menuIndex)
        {
            Transform currentMenu = transform.GetChild(menuIndex);
            currentMenu.GetChild(0).GetComponent<Image>().color = new Color(255, 255, 255, 0);

            while (currentMenu.GetChild(0).GetComponent<Image>().color.a < 255)
            {
                Color currentImageColour = currentMenu.GetChild(0).GetComponent<Image>().color;
                Color currentTextColor = currentMenu.GetChild(1).GetComponent<Text>().color;

                float fadeAmount = currentMenu.GetChild(0).GetComponent<Image>().color.a + (fadeSpeed * Time.deltaTime);
                Color currentImageFade = new Color(currentImageColour.r, currentImageColour.g, currentImageColour.b, fadeAmount);
                Color currentTextFade = new Color(currentTextColor.r, currentTextColor.g, currentTextColor.b, fadeAmount);

                currentMenu.GetChild(0).GetComponent<Image>().color = currentImageFade;
                currentMenu.GetChild(1).GetComponent<Text>().color = currentTextFade;
                yield return null;
            }

            StartCoroutine(WaitAndPrint());
        }

        private IEnumerator WaitAndPrint()
        {
            yield return new WaitForSeconds(delayTime[_currentMenuItem]);
            _currentChild.gameObject.SetActive(false);
            _currentMenuItem += 1;

            EnableNextScreen();
        }

        private void EnableNextScreen()
        {
            // If the menu we have currently is less than the length of timer values then we have more menus. 
            if (_currentMenuItem < delayTime.Length)
            {
                // Stop any fade that are running
                StopAllCoroutines();

                // Move to the next gameobject in the menu list. 
                _currentChild = transform.GetChild(_currentMenuItem);

                // Set it to active in the hierarchy 
                _currentChild.gameObject.SetActive(true);

                // Start our fades and start the wait script in case the user does not want to hit space
                StartCoroutine(FadeInObject(_currentMenuItem));
                StartCoroutine(WaitAndPrint());
            }

            else
            {
                // You could prob put a delay in here before switching scenes. 
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

                // Commented out as it's never assigned and will cause an error. I would put this in another UI script. 
                /*_thisCanvas.alpha = 0;
            _thisCanvas.blocksRaycasts = false;
            _thisCanvas.interactable = false;

            _inGameHUD.gameObject.SetActive(true);
            _inGameHUD.alpha = 1;
            _inGameHUD.blocksRaycasts = true;
            _inGameHUD.interactable = true;*/
            }
        }
    }
}