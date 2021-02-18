using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_script : MonoBehaviour {

	private Rigidbody2D rb;  // rb is used to enable and control the physics of the character.

	public float dashSpeed = 0f;    // Speed of the initial dash.
	public float runSpeed  = 8.5f;  // Movement speed when running (not the initial dash)
	public float walkSpeed = 3.5f;  // Maximum movement speed when walking.
	public float jumpHeight = 25f;  // Height of the peak of the jump.
	public float maxFallSpeed = -8.5f;  // Maximum fall speed without fast falling.
	private float horizontalMove = 0f;	 // How far analog stick is pressed on horizontal axis (between -1 and +1).
	private bool smashMode = false;  // Determines if normal atk button will do smashs or tilts, and if player will walk or run.
	
	private bool test = false;

	// Use this for initialization
	void Awake () {
		rb = gameObject.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		horizontalMove = Input.GetAxisRaw("Horizontal");

		if (horizontalMove >= -0.1f && horizontalMove <= 0.1f) {
			test = false;
		}

		/* if ((horizontalMove >= 0.1f && rb.velocity.x <= 0.1) || */
		/* 		(horizontalMove <= -0.1f && rb.velocity.x >= -0.1f)) { */
		/* 	test = false; */
		/* } */

		if (Input.GetKeyDown(KeyCode.A)) {
			smashMode = true;
		}
		else if (Input.GetKeyUp(KeyCode.A)) {
			smashMode = false;
		}

		if (horizontalMove != 0) {
			if (smashMode) {
				rb.velocity = new Vector2(horizontalMove * walkSpeed, rb.velocity.y);
			}
			else {
				if (!test) {
					rb.AddForce(new Vector2(dashSpeed * horizontalMove, rb.velocity.y), ForceMode2D.Impulse);
					Debug.Log(rb.velocity.x);
					test = true;
				}
				else {
					/* rb.velocity = new Vector2(horizontalMove * runSpeed, rb.velocity.y); */
					float x =  Mathf.Clamp(Abs(rb.velocity.x), runSpeed, dashSpeed);
					rb.velocity = new Vector2(x * horizontalMove, rb.velocity.y);
				}
			}
		}
		
		if (Input.GetKeyDown(KeyCode.Space)) {
			rb.velocity = new Vector2(rb.velocity.x, 0f);
			rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
		}

		if (rb.velocity.y < 0) {
			float y =  Mathf.Clamp(rb.velocity.y, maxFallSpeed, 0f);
			rb.velocity = new Vector2(rb.velocity.x, y);
		}
	}

	float Abs (float num) {
		if (num < 0) num = -num;
		return(num);
	}
}
