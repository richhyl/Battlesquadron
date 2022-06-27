using UnityEngine;
using System.Collections;

public class BulletDamageHandler : MonoBehaviour
{

	public int health = 1;
	public float invulnPeriod = 0;
	float invulnTimer = 0;
	int correctLayer;

	SpriteRenderer spriteRend;

	void Start()
	{
		correctLayer = gameObject.layer;

		// NOTE!  This only get the renderer on the parent object.
		// In other words, it doesn't work for children. I.E. "enemy01"
		spriteRend = GetComponent<SpriteRenderer>();

		if (spriteRend == null)
		{
			spriteRend = transform.GetComponentInChildren<SpriteRenderer>();

			if (spriteRend == null)
			{
				Debug.LogError("Object '" + gameObject.name + "' has no sprite renderer.");
			}
		}
	}

	void OnTriggerEnter2D()
	{
		health--;

		if (invulnPeriod > 0)
		{
			invulnTimer = invulnPeriod;
			gameObject.layer = 8;
		}
	}

	void Update()
	{
		if (health <= 0)
		{
			Die();
		}
	}


	void Die()
	{
		Destroy(gameObject);
	}

}
