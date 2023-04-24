using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{

    public TMPro.TMP_Text[] m_lines = new TMPro.TMP_Text[12];

    private const float kYSpacing = 40.0f;
    private const float kAngleOutSpeed = 3.0f;
    private const float kAngleInSpeed = 0.05f;
    private const float kSinusAmp = 100.0f;

    private const float kArrivalTime = 1.2f;            // in seconds
    private const float kArrivalStartX = -1200.0f;

    private float m_clock;
    private float m_xStart;
    private float m_angle;
    private float m_yStart;

    // Start is called before the first frame update
    void Start()
    {
        var music = FindObjectOfType<m68kEmulation>();
        if (music != null)
            music.MusicRestart(2); // gameover music

        m_lines[0].text = "BATTLE SQUADRON";
        m_lines[1].text = "BRAVEST AVIATORS";
        m_lines[2].text = " 1. MPB 1000000";
        m_lines[3].text = " 2. TBL 0800000";
        m_lines[4].text = " 3. R.K 0600000";
        m_lines[5].text = " 4. Y.Z 0400000";
        m_lines[6].text = " 5. T.Z 0200000";
        m_lines[7].text = " 6. VBN 0080000";
        m_lines[8].text = " 7. P.L 0060000";
        m_lines[9].text = " 8. NCP 0040000";
        m_lines[10].text = " 9. T.J 0020000";
        m_lines[11].text = "10. LMP 0001000";

        RectTransform rt = m_lines[0].GetComponent<RectTransform>();
        m_yStart = rt.localPosition.y;

        m_angle = 3.1415926f * 0.5f;

    }

    private float smoothStep(float x)
    {
        return 3 * x * x - 2.0f * x * x * x;
    }

    // Update is called once per frame
    void Update()
    {

        float xOffset = 0.0f;
        float amp = kSinusAmp;
        if ( m_clock < kArrivalTime )
        {
            float alpha = m_clock / kArrivalTime;
//            amp = kSinusAmp * smoothStep(alpha);
//             alpha = 1.0f - alpha;       // [1..0]
//             alpha = alpha * alpha;   // smooth [1..0]
            xOffset = kArrivalStartX * (1.0f - smoothStep(alpha));
        }

/*
        if (( m_clock >= 0.5f) && ( m_clock < 1.5f ))
        {
            float alpha = (m_clock-0.5f) / 1.0f;
            amp = kSinusAmp * smoothStep(alpha);
        }
*/

        float angle = m_angle;
        float yPos = m_yStart;
        for (int i=0;i<12;i++)
        {
            if (m_lines[i] != null)
            {
                RectTransform rt = m_lines[i].GetComponent<RectTransform>();
                rt.localPosition = new Vector3(amp * Mathf.Cos(angle)+xOffset, yPos, 0);
            }
            angle += kAngleInSpeed;

            yPos -= kYSpacing;
            if (i == 1)
                yPos -= 20.0f;
        }

        m_angle += Time.deltaTime * kAngleOutSpeed;

        m_clock += Time.deltaTime;

        if ( m_clock > 2.0f)        // avoid quiting high scores immediately if user was clicking like crazy to kill enemies right before
        {
            if (Input.anyKey)
                SceneManager.LoadScene(0);
        }
    }
}
