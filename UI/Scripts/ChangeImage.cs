using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeImage : MonoBehaviour
{
    public Image Player1oldImage;
    public Sprite Player1newSprite;
    public Image Player2oldImage;
    public Sprite Player2newSprite;
    public Image FXoldImage;
    public Sprite FXnewSprite;
    public Image MusicoldImage;
    public Sprite MusicnewSprite;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Player1oldImage.sprite = Player1newSprite;
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            Player2oldImage.sprite = Player2newSprite;
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            FXoldImage.sprite = FXnewSprite;
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            MusicoldImage.sprite = MusicnewSprite;
            AudioListener.volume = 0;
        }

    }
}
