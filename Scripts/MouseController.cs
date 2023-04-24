using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    public float CameraDistance = 20.0f; // TODO: measure the actual camera distance at start to ease camera pos tweaking?

    const float kMaxShipTilt = 45.0f; // degrees

    float m_CurrentTiltAngle = 0.0f;

    Quaternion m_OriginalRotation;
    Vector3 m_PreviousPosition;

    public Camera mainCamera;

    public GameObject shotBlue;
    public GameObject shotGreen;
    public GameObject shotRed;
    public GameObject shotYellow;
    public GameObject m_ShipRenderer;

    public Transform shotSpawnL;
    public Transform shotSpawnR;
    public Boundary boundary;
    public float fireRate = 0.5f;
    float currentFireRate;
    private float nextFire = 0.0f;

    public GameObject shipMesh;

    public float invulnTime;
    public float blinkoffTime;

    private float curInvuln = 0.0f;

    private float curBlinkOff = 0.0f;
    public static MouseController gPlayer;

    private uint weaponLevel = 0;
    enum WeaponType { Yellow, Red, Blue, Green }; // TODO: Maybe make WeaponDropMovement forcibly match this?
    private WeaponType weaponType = WeaponType.Blue;

    private uint curShot = 0;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        gPlayer = this;

        m_CurrentTiltAngle = 0.0f;
        m_OriginalRotation = transform.localRotation;
        m_PreviousPosition = transform.position;

        currentFireRate = fireRate;

        // Mouse input coordinates range from 0,0 to Screen.width,Screen.height
        // Some magic offsetting based on those to have sensible limits for the movement.
        // TODO: maybe the boundaries should be done on the game object world space instead of control input space...
        boundary.xMin = 100;
        boundary.xMax = Screen.width - 100;
        boundary.yMin = 100;
        boundary.yMax = Screen.height;
    }

    private void FixedUpdate()
    {
        // Make the ship follow mouse position
        //transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, CameraDistance));

        Vector3 m_pos = new Vector3
        (
            Mathf.Clamp(Input.mousePosition.x, boundary.xMin, boundary.xMax),
            Mathf.Clamp(Input.mousePosition.y, boundary.yMin, boundary.yMax),
            CameraDistance
        );
        Vector3 tmpPos = Camera.main.ScreenToWorldPoint(m_pos);
        tmpPos.y = 0.0f;

        transform.position = tmpPos;

        // Debug.Log(m_pos);

        // Calculate ship tilt from horizontal velocity
        float horizontalVelocity = (m_PreviousPosition.x - transform.position.x) * 150.0f;
        horizontalVelocity = horizontalVelocity > 0.0f ? Mathf.Min(horizontalVelocity, kMaxShipTilt) : Mathf.Max(horizontalVelocity, -kMaxShipTilt);
        m_CurrentTiltAngle = m_CurrentTiltAngle * 0.75f + horizontalVelocity * 0.25f;

        m_PreviousPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = m_OriginalRotation;
        transform.Rotate(Vector3.forward, m_CurrentTiltAngle);

        if (mainCamera)
        {
            Vector3 pos = mainCamera.transform.position;
            pos.x = pos.x * 0.9f + 0.1f * (transform.position.x * 0.5f);
            mainCamera.transform.position = pos;
        }

        if (m_ShipRenderer.activeSelf)
        {
            if (Input.GetButtonDown("DebugWeaponType"))
            {
                weaponType = (WeaponType)(((int)weaponType + 1) % 4);
            }
            if (Input.GetButtonDown("DebugPowerup"))
            {
                weaponLevel++;
            }


            if (Input.GetButtonDown("Fire1"))
            {
                curShot = 0;
            }

            if (Input.GetButton("Fire1") && Time.time >= nextFire)
            {
                nextFire = Time.time + currentFireRate;
                if (weaponType == WeaponType.Blue) FireBlue();
                if (weaponType == WeaponType.Green) FireGreen();
                if (weaponType == WeaponType.Red) FireRed();
                if (weaponType == WeaponType.Yellow) FireYellow();

                curShot++;
            }
        }

        curBlinkOff -= Time.deltaTime;

        if (curBlinkOff <= 0.0f && curInvuln >= 0.0f)
        {
            shipMesh.GetComponent<MeshRenderer>().enabled = !shipMesh.GetComponent<MeshRenderer>().enabled;

        }

        if (curInvuln > 0.0f)
        {
            gameObject.GetComponent<MeshCollider>().enabled = false;
            curInvuln -= Time.deltaTime;

            if (curBlinkOff <= 0.0f)
            {
                curBlinkOff = blinkoffTime;
            }
        }
        else
        {
            shipMesh.GetComponent<MeshRenderer>().enabled = true;
            gameObject.GetComponent<MeshCollider>().enabled = true;
        }

    }


    private void OnCollisionEnter(Collision collision)
    {
        if (curInvuln <= 0.0f)
        {
            if (collision.gameObject.layer == 8 || collision.gameObject.layer == 9)
            {
                GetComponent<PlayerState>().DecreaseLives();
                //                Debug.Log("You died!");
                weaponLevel = 0;
                if (collision.gameObject.layer == 9) Destroy(collision.gameObject);
                curInvuln = invulnTime;
            }
        }
        if (collision.gameObject.layer == 10)
        {
            uint weaponIndex = collision.gameObject.GetComponent<WeaponDropMovement>().colorIndex;
            weaponLevel++;
            weaponType = (WeaponType)weaponIndex;
            Destroy(collision.gameObject);
        }

    }

    // The bullet types are fully separated as there can be plenty of handtuning for each to make them match original
    private void FireRed()
    {
        for (int i = 0; i < weaponLevel + 1; i++)
        {
            Instantiate(shotRed, shotSpawnL.position, shotSpawnL.rotation * Quaternion.AngleAxis(12 * i, Vector3.right)).GetComponent<BulletController>().player = gameObject;
            Instantiate(shotRed, shotSpawnR.position, shotSpawnR.rotation * Quaternion.AngleAxis(-12 * i, Vector3.right)).GetComponent<BulletController>().player = gameObject;
        }
        currentFireRate = fireRate;
    }

    //TODO: Some sort of nice wiggle
    private void FireYellow()
    {
        Instantiate(shotYellow, shotSpawnL.position, shotSpawnL.rotation).GetComponent<BulletController>().player = gameObject;
        GameObject right = Instantiate(shotYellow, shotSpawnR.position, shotSpawnR.rotation);
        right.GetComponent<BulletController>().player = gameObject;
        right.GetComponent<FireMovement>().phase = -1;
        currentFireRate = (fireRate * 2) / (weaponLevel + 2);
    }

    private void FireGreen()
    {
        uint shotspread = curShot % (weaponLevel + 1);
        Instantiate(shotGreen, shotSpawnL.position, shotSpawnL.rotation * Quaternion.AngleAxis(4 * shotspread, Vector3.right)).GetComponent<BulletController>().player = gameObject;
        Instantiate(shotGreen, shotSpawnR.position, shotSpawnR.rotation * Quaternion.AngleAxis(-4 * shotspread, Vector3.right)).GetComponent<BulletController>().player = gameObject;

        currentFireRate = (fireRate * 2) / (weaponLevel + 2);
    }

    private void FireBlue()
    {
        for (int i = 0; i < weaponLevel + 1; i++)
        {
            Instantiate(shotBlue, shotSpawnL.position + new Vector3(-0.5f * Mathf.Floor((float)i / 2), 0f, 0f), shotSpawnL.rotation * Quaternion.AngleAxis(180 * (i % 2), Vector3.right)).GetComponent<BulletController>().player = gameObject;
            Instantiate(shotBlue, shotSpawnR.position + new Vector3(0.5f * Mathf.Floor((float)i / 2), 0f, 0f), shotSpawnR.rotation * Quaternion.AngleAxis(180 * (i % 2), Vector3.right)).GetComponent<BulletController>().player = gameObject;
        }
        currentFireRate = fireRate;

    }
}

[System.Serializable]
public class Boundary
{
    public float xMin, xMax, yMin, yMax;
}
