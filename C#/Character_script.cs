using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_script : MonoBehaviour {

	private Rigidbody2D rb;  // rb is used to enable and control the physics of the character.

	public float dashSpeed = 14f;   // Speed of the initial dash.
	public float runSpeed  = 8.5f;  // Movement speed when running (not the initial dash)
	public float walkSpeed = 3.5f;  // Maximum movement speed when walking.
	public float jumpHeight = 25f;  // Height of the peak of the jump.
	public float maxFallSpeed = -8.5f;  // Maximum fall speed without fast falling.
	public int maxNbJumps = 2;      // Number of times the player can jump before touching the ground.

	private int jumpsUsed = 0;      // Number of jumps performed before touching the ground.
	private float horizontalMove = 0f;	 // How far analog stick is pressed on horizontal axis (between -1 and +1).
	private bool smashMode = false;  // Determines if normal atk button will do smashs or tilts, and if player will walk or run.
	
	public bool isGrounded = false;
	private bool ableToDash = true;

	// Use this for initialization
	void Awake () {
		rb = gameObject.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		horizontalMove = Input.GetAxisRaw("Horizontal");

		if (isGrounded) {
			jumpsUsed = 0;
		}

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
				if (ableToDash) {
					rb.AddForce(new Vector2(dashSpeed * horizontalMove, 0f), ForceMode2D.Impulse);
					ableToDash = false;
				}
				else {
					float x =  Mathf.Clamp(Abs(rb.velocity.x), runSpeed, dashSpeed);
					rb.velocity = new Vector2(x * horizontalMove, rb.velocity.y);
				}
			}
		}
		
		if (Input.GetKeyDown(KeyCode.Space)) {
			if (jumpsUsed < maxNbJumps) {
				jumpsUsed++;
				isGrounded = false;
				rb.velocity = new Vector2(rb.velocity.x, 0f);
				rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
			}
		}

		if (rb.velocity.y < 0) {
			float y =  Mathf.Clamp(rb.velocity.y, maxFallSpeed, 0f);
			rb.velocity = new Vector2(rb.velocity.x, y);
		}
	}

	/* void OnCollisionEnter2D (Collision2D collision) { */
	/* 	if (collision.gameObject.CompareTag("Ground") && rb.velocity.y <= 0.01f) { */
	/* 		Debug.Log("uuu"); */
	/* 		isGrounded = true; */
	/* 	} */
	/* } */

	/* void OnCollisionExit2D (Collision2D collision) { */
	/* 	Debug.Log("iii"); */
	/* 	if (collision.gameObject.CompareTag("Ground")) { */
	/* 		isGrounded = false; */
	/* 	} */
	/* } */

	float Abs (float num) {
		if (num < 0) num = -num;
		return(num);
	}
}
