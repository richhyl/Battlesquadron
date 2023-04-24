using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BasicEnemyController : MonoBehaviour
{
    public int health;
    public int score = 100;
    public GameObject m_Bullet;
    public GameObject m_Explosion;

    private const float kBlinkTime = 0.2f;

    private float m_clock;

    public float m_initialSleepTime;
    public float m_fireRate;
    public float m_fireSpeed;
    public int m_burstBullets;
    public int m_beforeBurstCount;
    private int m_beforeBurst;

    private float m_burstClock;
    private Material m_originalMaterial;
    private Material m_blinkMaterial;
    private MeshRenderer m_meshRenderer;
    private float m_blinkTimer;

    // Start is called before the first frame update
    void Start()
    {
        m_clock = m_initialSleepTime;
        m_beforeBurst = m_beforeBurstCount;

        MeshRenderer[] mrs = GetComponentsInChildren<MeshRenderer>();
        if (mrs.Length > 0)
        {
            m_meshRenderer = mrs[0];
            m_originalMaterial = mrs[0].material;
            m_blinkMaterial = Instantiate(mrs[0].material);
            m_blinkMaterial.SetColor("_BaseColor", Color.white);
            m_blinkMaterial.SetFloat("_Metallic", 1.0f);
            m_blinkMaterial.SetTexture("_BaseMap", Texture2D.whiteTexture);
        }
    }
    // Update is called once per frame
    void Update()
    {



        if(health <=  0)
        {
            Instantiate(m_Explosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
            return;
        }

        if ( m_blinkTimer > 0.0f)
        {
            m_blinkTimer -= Time.deltaTime;
            if ( m_blinkTimer <= 0.0f)
            {
                m_meshRenderer.material = m_originalMaterial;
            }
        }


        m_clock -= Time.deltaTime;
        m_burstClock -= Time.deltaTime;



        if (m_Bullet != null)
        {
            MouseController player = MouseController.gPlayer;
            if (player != null)
            {
                if (m_clock <= 0.0f)
                {
                    m_beforeBurst--;
                    if (m_beforeBurst <= 0)
                    {
                        for (int i = 0; i < m_burstBullets; i++)
                        {
                            float a = 3.415926f * 2.0f * (float)i / (float)m_burstBullets;

                            Vector3 dir = new Vector3(Mathf.Cos(a), transform.position.y, Mathf.Sin(a));

                            var obj = Instantiate(m_Bullet, transform.position, Quaternion.identity);
                            EnemiBulletController bullet = obj.GetComponent<EnemiBulletController>();
                            bullet.Launch(transform.position, dir, m_fireSpeed);
                        }
                        m_beforeBurst = m_beforeBurstCount;
                    }
                    else
                    {
                        var obj = Instantiate(m_Bullet, transform.position, Quaternion.identity);
                        EnemiBulletController bullet = obj.GetComponent<EnemiBulletController>();

                        Vector3 dir = player.transform.position - transform.position;
                        dir.Normalize();

                        bullet.Launch(transform.position, dir, m_fireSpeed);
                    }
                    m_clock = m_fireRate;
                }
            }
        }
    }

    public void TakeDamage(int amount, GameObject player)
    {
        health -= amount;

        m_meshRenderer.material = m_blinkMaterial;
        m_blinkTimer = kBlinkTime;

        var playerState = player.GetComponent<PlayerState>();
        if (health > 0)
        { 
            GetComponent<AudioSource>().Play();
            playerState.AddScore(10);
        }
        else
        {
            playerState.AddScore(score);
        }
    }
}
