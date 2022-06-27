using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretMovement : MonoBehaviour
{
    public float turnSpeed01 = 0.25f;

    float angle01 = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MouseController player = MouseController.gPlayer;
        Vector3 dir = player.transform.position - transform.position;
        dir.y = 0.0f;

        angle01 = angle01 - Mathf.Floor(angle01);

        float targetAngleRadians = -Mathf.Atan2(dir.z, dir.x);
        float targetAngle01 = targetAngleRadians / (Mathf.PI * 2.0f);
        targetAngle01 = targetAngle01 - Mathf.Floor(targetAngle01);

        float deltaAngle01 = targetAngle01 - angle01;
        if (deltaAngle01 > 0.5f)
            deltaAngle01 -= 1.0f;
        if (deltaAngle01 < -0.5f)
            deltaAngle01 += 1.0f;

        float maxTurn = turnSpeed01 * Time.deltaTime;
        angle01 = angle01 + Mathf.Clamp(deltaAngle01, -maxTurn, maxTurn);

        transform.rotation = Quaternion.AngleAxis(angle01 * 360.0f, new Vector3(0.0f, 1.0f, 0.0f));
    }
}
