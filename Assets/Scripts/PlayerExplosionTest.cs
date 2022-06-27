using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExplosionTest : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetBool("Explosion", true);
            Destroy(gameObject, 0.5f);
        }
    }
}
