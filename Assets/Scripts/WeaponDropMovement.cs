using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDropMovement : MonoBehaviour
{
    public float VerticalSpeed = 2.0f;
    public float MaxHorizontalPosition = 25.0f;
    public float HorizontalMovementSpeed = 1.0f;
    public GameObject ParentShip;

    float m_TargetPosX;

    Material m_Material;
    public uint colorIndex = 0;
    Color[] kColors = {Color.yellow, Color.red, Color.blue, Color.green };

    // Start is called before the first frame update
    void Start()
    {
        m_TargetPosX = MaxHorizontalPosition;
        m_Material = GetComponent<Renderer>().material;
        colorIndex = 0;
        m_Material.SetColor("Color_c0eede3513454935a4bd964b13d15275", kColors[colorIndex]);
        m_Material.SetFloat("Vector1_d43d0013146d47779079d6eca7481911", 0.03f);
    }

    void ChangeColor()
    {
        colorIndex++;
        if (colorIndex >= kColors.Length)
            colorIndex = 0;

        m_Material.SetColor("Color_c0eede3513454935a4bd964b13d15275", kColors[colorIndex]);
    }
    // Update is called once per frame
    void Update()
    {
        if (ParentShip == null)
        {
            m_Material.SetFloat("Vector1_d43d0013146d47779079d6eca7481911", 0.6f);
            float horizontalSpeed  = (m_TargetPosX > 0) ? HorizontalMovementSpeed : -HorizontalMovementSpeed;
            transform.parent.Translate(Vector3.left * (Time.deltaTime * horizontalSpeed));
            transform.parent.Translate(Vector3.forward * (Time.deltaTime * VerticalSpeed));

            if (m_TargetPosX > 0)
            {
                if ((m_TargetPosX - transform.parent.position.x) < 0.0f)
                { 
                    m_TargetPosX = -m_TargetPosX;
                    ChangeColor();
                }
            }
            else
            {
                if ((m_TargetPosX - transform.parent.position.x) > 0.0f)
                { 
                    m_TargetPosX = -m_TargetPosX;
                    ChangeColor();
                }
            }
        }
    }
}
