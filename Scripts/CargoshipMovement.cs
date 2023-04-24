using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoshipMovement : MonoBehaviour
{
    public float VerticalSpeed = 2.0f;
    public float HorizontalMovementRange = 1.0f;
    public float HorizontalMovementSpeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.parent.Translate(Vector3.left * Mathf.Sin(Time.time * HorizontalMovementSpeed) * HorizontalMovementRange);
        transform.parent.Translate(Vector3.forward * (Time.deltaTime * VerticalSpeed));
    }
}
