using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    private float m_Countdown = 3;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        m_Countdown -= Time.deltaTime;
        if (m_Countdown <= 0)
        {
            Destroy(gameObject);
        }
    }
}
