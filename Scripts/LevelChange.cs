using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelChange : MonoBehaviour
{
    public GameObject GameOverOverlay;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnTriggerEnter(Collider collider)
    {
        // If the player collided with the level exit platform, engage game over logic
        if(collider.gameObject.layer == 6)
        {
            collider.gameObject.SetActive(false);
            GameOverOverlay.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
