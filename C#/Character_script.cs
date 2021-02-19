using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_script : MonoBehaviour {

	private Rigidbody2D rb;  // rb is used to enable and control the physics of the character.

	public float dashSpeed = 16f;         // Speed of the initial dash.
	public float runSpeed  = 9.5f;        // Movement speed when running (not the initial dash)
	public float walkSpeed = 4.5f;        // Maximum movement speed when walking.
	public float fullHopHeight = 25f;     // Height of the normal jump.
	public float shortHopHeight = 18f;    // Height of the short jump.
	public float doubleJumpHeight = 27f;  // Height of the double jump.
	public int maxNbJumps = 2;            // Number of times the player can jump before touching the ground.
	public float maxFallSpeed = -8.5f;    // Maximum fall speed without fast falling.
	public float airAccel = 14.5f;        // Acceleration when moving in the air.
	public float airSpeed = 7.75f;        // Maximum speed in the air.
	public bool isGrounded = false;       // Determines if the player is touching the ground.

	private float horizontalMove = 0f;	  // Position of the analog stick x-axis (between -1 and +1).
	private bool ableToDash = true;       // Determines if the player does the initial dash when running.
	private bool smashMode = false;       // Determines if normal atk button will do smashs or tilts, and if player will walk or run.
	private int jumpsUsed = 0;            // Number of jumps performed before touching the ground.


	// Use this for initialization
	void Awake () {
		rb = gameObject.GetComponent<Rigidbody2D>();
	}
	

	// Update is called once per frame
	void Update () {
		horizontalMove = Input.GetAxisRaw("Horizontal");  // Gets position of analog stick on x-axis

		// When smash mode is activated, you walk instead of dash, and do smash attacks instead of tilts.
		if (Input.GetKeyDown(KeyCode.A)) smashMode = true;
		else if (Input.GetKeyUp(KeyCode.A)) smashMode = false;

		// Calls the Jump function when spacebar is pressed and you still have jumps left.
		if ((Input.GetKeyDown(KeyCode.Space)) && (jumpsUsed < maxNbJumps)) {
			if (isGrounded && !smashMode) Jump(0);      // Perform a standard jump.
			else if (isGrounded && smashMode) Jump(1);  // Perform a short jump.
			else if (!isGrounded) Jump(2);              // Perform a double jump.
		}
	}


	void FixedUpdate() {
		if (isGrounded) jumpsUsed = 0;  // Resets the number of jumps performed when touching the ground.

		if (horizontalMove != 0) {  // If the player is moving.
			if (isGrounded) {  // If the player is on the ground.
				if (smashMode) {  // Physics when walking.
					rb.velocity = new Vector2(horizontalMove * walkSpeed, rb.velocity.y);
				}
				else {  // Physics when running.
					if (ableToDash) {  // Performs a dash at the start of the run.
						rb.AddForce (new Vector2(dashSpeed * horizontalMove, 0f), ForceMode2D.Impulse);
						ableToDash = false;
					}
					else {  // will slow down naturaly due to the friction and keep the player at running speed.
						float x =  Mathf.Clamp(Abs(rb.velocity.x), runSpeed, dashSpeed);
						rb.velocity = new Vector2(x * horizontalMove, rb.velocity.y);
					}
				}
			}
			else {  // If the player is in the air.
				// Slowly accelerate when in the air.
				rb.AddForce (new Vector2(horizontalMove * airAccel, rb.velocity.y), ForceMode2D.Force);
				// Speed will not go faster than the value airSpeed variable.
				float x = Mathf.Clamp(rb.velocity.x, -airSpeed, airSpeed);
				rb.velocity = new Vector2(x, rb.velocity.y);
			}
		}
		
		if (rb.velocity.y < 0) {  // Limits the maximum speed of the player when falling down.
			float y =  Mathf.Clamp(rb.velocity.y, maxFallSpeed, 0f);
			rb.velocity = new Vector2(rb.velocity.x, y);
		}
	}


	void Jump (int jumpType) {  // A standard Jump.
		jumpsUsed++;
		isGrounded = false;
		rb.velocity = new Vector2(rb.velocity.x, 0f);
		float jumpHeight = new float[] {fullHopHeight, shortHopHeight, doubleJumpHeight}[jumpType];
		rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
	}


	float Abs (float num) {  // Returns the absolute value of a float.
		if (num < 0) num = -num;
		return(num);
	}
}
