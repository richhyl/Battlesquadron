using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireMovement : MonoBehaviour
{
    Rigidbody body;
    // Start is called before the first frame update
    public int phase = 1;
    void Start()
    {
        body = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        

        body.velocity = Quaternion.AngleAxis(phase * Mathf.Sin(Time.time*10f)*2.4f,Vector3.up) * body.velocity;
    }
}
