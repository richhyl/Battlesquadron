using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiBulletController : MonoBehaviour
{

    private Vector3 m_pos;
    private Vector3 m_dir;
    private float m_speed;
    private float m_timer;

    private const float kBulletLifeTime = 4.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_timer = kBulletLifeTime;
    }

    public void Launch(Vector3 pos, Vector3 dir, float speed)
    {
        m_pos = pos;
        m_dir = dir;
        m_speed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        m_pos = m_pos + m_dir * m_speed * Time.deltaTime;
        transform.position = m_pos;

        m_timer -= Time.deltaTime;
        if (m_timer <= 0)
            Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 0)
        {
            Destroy(gameObject);
        }
    }

}
