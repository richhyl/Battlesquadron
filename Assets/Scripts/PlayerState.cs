using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public TMPro.TMP_Text LivesUI;
    public TMPro.TMP_Text NovaBombsUI;
    public TMPro.TMP_Text ScoreUI;
    public GameObject GameOverOverlay;
    public GameObject m_Explosion;
    public GameObject m_ShipRenderer;

    private const float kDeadTimer = 2.0f;

    private int lives = 3;
    private int novaBombs = 3;
    private int score = 0;

    private float m_deadTimer;


    public void DecreaseLives()
    {
       
        lives--;
        LivesUI.text = lives.ToString();

        Instantiate(m_Explosion, transform.position, Quaternion.identity);
        m_deadTimer = kDeadTimer;
        m_ShipRenderer.SetActive(false);

    }

    public void AddScore(int addedScore)
    {
        score += addedScore;
        ScoreUI.text = score.ToString();
        while(ScoreUI.text.Length < 8)
        {
            ScoreUI.text = "0" + ScoreUI.text;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        LivesUI.text = lives.ToString();
        NovaBombsUI.text = novaBombs.ToString();
        ScoreUI.text = score.ToString("00000000");
    }

    private void Update()
    {
        if ( m_deadTimer > 0.0f)
        {
            m_deadTimer -= Time.deltaTime;
            if ( m_deadTimer <= 0.0f)
            {
                m_ShipRenderer.SetActive(true);
                if (lives <= 0)
                {
                    gameObject.SetActive(false);
                    GameOverOverlay.SetActive(true);
                }
            }

        }
    }

}
