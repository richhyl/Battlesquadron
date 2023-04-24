using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Friendly bullets or whatnot damaging projectiles
public class BulletController : MonoBehaviour
{
    public float speed;
    public float lifetime;
    public int damage;
    public GameObject player; // the player that shot this bullet

    private float timeAlive = 0.0f;

    void Start()
    {
        Vector3 tmpVec = transform.forward;
        tmpVec.y = 0f;
        tmpVec.Normalize();
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z); //Make the bullets always point forwards like in original
        GetComponent<Rigidbody>().velocity = tmpVec * speed;
    }

    private void Update()
    {
        timeAlive += Time.deltaTime;
        if(timeAlive > lifetime)
        {
            Destroy(gameObject);
            return;
        }        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 0)
        {
            Destroy(gameObject);
        }
        if(collision.gameObject.layer == 8)
        {
            collision.gameObject.GetComponent<BasicEnemyController>().TakeDamage(damage, player);
            Destroy(gameObject);
        }
    }
}
