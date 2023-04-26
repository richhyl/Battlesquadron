using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tetstilemapscroller : MonoBehaviour
{
    public float startSpeed;
    public float slowSpeed;
    public float slowDownTime;
    private float elapsedTime;
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
        elapsedTime = 0f;
    }

    void Update()
    {
        if (elapsedTime < slowDownTime)
        {
            transform.Translate(Vector3.up * startSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.up * slowSpeed * Time.deltaTime);
        }

        elapsedTime += Time.deltaTime;
    }
}