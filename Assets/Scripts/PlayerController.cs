using UnityEngine;
using System.Collections;

[System.Serializable]
public class Boundary1
{
	public float xMin, xMax, yMin, yMax;
}

public class PlayerController : MonoBehaviour
{
	public float speed;
	public Boundary1 boundary;
	//public Animator animator;


    void FixedUpdate ()
	{
		//animator.SetFloat("Horizontal", Input.GetAxis("Horizontal"));
		
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		Vector2 movement = new Vector2 (moveHorizontal, moveVertical);
		GetComponent<Rigidbody2D>().velocity = movement * speed;
		
		GetComponent<Rigidbody2D>().position = new Vector2
		(
            Mathf.Clamp(GetComponent<Rigidbody2D>().position.x, boundary.xMin, boundary.xMax), Mathf.Clamp(GetComponent<Rigidbody2D>().position.y, boundary.yMin, boundary.yMax)
		);
		
		//GetComponent<Rigidbody>().rotation = Quaternion.Euler (0.0f, 0.0f, GetComponent<Rigidbody>().velocity.x * -tilt);
	}
}
