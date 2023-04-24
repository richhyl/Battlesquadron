using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class StarfieldController : MonoBehaviour
{
    VisualEffect vfx;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnEnable()
    {
        // setup initial state
        vfx = GetComponent<VisualEffect>();
        // vfx.Simulate(1/30.0f, 40);

        timeAccum = 0.0f;
    }

    float timeAccum;
    const float maxTime = 3.0f;

    // Update is called once per frame
    void Update()
    {
        timeAccum += Time.deltaTime;

        if (timeAccum < maxTime)
        {
            float t = 1 - timeAccum / maxTime;
            float x = Mathf.Clamp01(t * 2);
            float a = Mathf.Clamp01((3 - 2 * t) * t * t);
            a = Mathf.Pow(a, 3.0f);
            vfx.Simulate(Time.deltaTime * (a * 12.0f + 1.0f), 1);
        }
    }
}
