using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tetstilemapscroller : MonoBehaviour
{
    public float speed;
    private Vector3 StartPosition;
    private void Start()
    {
        StartPosition = transform.position;
    }
    void Update()
    {

        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }
}
