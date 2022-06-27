using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFighterMovement : MonoBehaviour
{
    public float VerticalSpeed = 3.0f;
    public float HorizontalMovementRange = 1.0f;
    public float HorizontalMovementSpeed = 1.0f;

    private float m_clock;

    Quaternion m_OriginalRotation;
    float m_OriginalX;

    // Start is called before the first frame update
    void Start()
    {
        m_OriginalRotation = transform.localRotation;
        m_OriginalX = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        float offset = Mathf.Sin(m_clock * HorizontalMovementSpeed);        // each fighter have its own clock so they aren't in sync
        var pos = transform.position;
        pos.x = m_OriginalX + (offset * HorizontalMovementRange);
        pos.y = 0f;
        transform.position = pos;
        transform.Translate(Vector3.forward * (Time.deltaTime * VerticalSpeed));

        float rotation = Mathf.Sin(m_clock * HorizontalMovementSpeed +  Mathf.PI / 2.0f);
        transform.localRotation = m_OriginalRotation;
        transform.Rotate(Vector3.forward, rotation * 45.0f);

        m_clock += Time.deltaTime;
    }
}
