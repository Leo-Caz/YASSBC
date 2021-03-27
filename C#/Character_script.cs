using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_script : MonoBehaviour {

	private Rigidbody2D rb;  // rb is used to enable and control the physics of the character.

	public float dashSpeed = 16f;         // Speed of the initial dash.
	public float runAccel = 4f;           // Acceleration at the start of a run.
	public float runSpeed  = 9.5f;        // Movement speed when running (not the initial dash)
	public float walkSpeed = 4.5f;        // Maximum movement speed when walking.
	public float fullHopHeight = 25f;     // Height of the normal jump.
	public float shortHopHeight = 18f;    // Height of the short jump.
	public float doubleJumpHeight = 23f;  // Height of the double jump.
	public int maxNbJumps = 2;            // Number of times the player can jump before touching the ground.
	public float maxFallSpeed = -8.5f;    // Maximum fall speed without fast falling.
	public float fastFallSpeed = -14.5f;  // Speed of the player when performing a fast-fall.
	public float airAccel = 14.5f;        // Acceleration when moving in the air.
	public float airSpeed = 7.75f;        // Maximum speed in the air.
	public float normalDrag = 2.6f;       // Value of the regular linear drag.
	public float airDodgeDrag = 2.6f;     // Value of the linear drag during an air dodge.
	public float airDodgeDist = 0f;       // Distance of the air dodge.
	public float gravity = 0f;            // Gravity of the character.

	public bool isGrounded = false;       // Determines if the player is touching the ground.
	public bool insidePlateform = false;  // Determines if the player is inside a platform when going through.
	public bool isFastFalling = false;    // Determines if the player performes a fast-fall or not.

	private float horizontalMove = 0f;	  // Position of the analog stick x-axis (between -1 and +1).
	private float verticalMove = 0f;	  // Position of the analog stick y-axis (between -1 and +1).
	private bool ableToDash = true;       // Determines if the player does the initial dash when running.
	private bool isWalking = false;       // If set to true, the player will walk intead of run.
	private int jumpsUsed = 0;            // Number of jumps performed before touching the ground.
	private bool isAirDodging = false;    // Determines if the player is in air dodge state.
	private bool ableToAirDodge = true;   // Determines if the player is able to perform an air dodge.
	private bool dirAirDodge = false;     // Determines if the player performed a directional AD or a normal one.


	// Use this for initialization
	void Awake () {
		rb = gameObject.GetComponent<Rigidbody2D>();
	}
	

	// Update is called once per frame
	void Update () {
		// Analog stick position
		horizontalMove = Input.GetAxisRaw("Horizontal");  // Gets position of analog stick on x-axis
		verticalMove = Input.GetAxisRaw("Vertical");  // Gets position of analog stick on x-axis

		if (Abs(horizontalMove) <= 0.1f) {
			ableToDash = true;
		}

		// Walking
		if (Input.GetKeyDown(KeyCode.LeftShift)) {
			isWalking = true;
		}
		else if (Input.GetKeyUp(KeyCode.LeftShift)) {
			isWalking = false;
		}

		// Jumping.
		if ((Input.GetKeyDown(KeyCode.Space)) && (jumpsUsed < maxNbJumps)) {
			if (isGrounded && !isWalking) Jump(0);      // Perform a standard jump.
			else if (isGrounded && isWalking) Jump(1);  // Perform a short jump.
			else if (!isGrounded) Jump(2);              // Perform a double jump.
		}

		// Airdodging.
		if (Input.GetKeyDown(KeyCode.N) && !isGrounded) {
			isAirDodging = true;
		}

		// Dropping of the plateform.
		if (Input.GetKeyDown(KeyCode.X)) {
			Physics2D.IgnoreLayerCollision(10, 9);  // Deactivate collisions with plateforms.
		}
		else if (!insidePlateform) {
			Physics2D.IgnoreLayerCollision(10, 9, false);  // Reactivate collisions when out of platform.
		}
		
		// Fast-fall.
		if (rb.velocity.y < 0.5f && Input.GetKeyDown(KeyCode.X)) {
			isFastFalling = true;
		}
	}


	void FixedUpdate() {
		if (isGrounded) {
			jumpsUsed = 0;  // Resets the number of jumps performed when touching the ground.
			isFastFalling = false; // Deactivates the fast-fall.
			ableToAirDodge = true;
			if (isAirDodging) {
				isAirDodging = false;  // Cancles the air dodge when landing to allow for combos.
				rb.gravityScale = gravity;  // Reactivate gravity.
				rb.drag = normalDrag;  // Set the linear drag to the regular value.
			}
		}

		if (isAirDodging) {
			if (ableToAirDodge) {
				ableToAirDodge = false;
				// Deactivate hurbox

				if (Abs(horizontalMove) >= 0.1f || Abs(verticalMove) >= 0.1f) {
					rb.velocity = new Vector2(0f, 0f);
					dirAirDodge = true;
					rb.gravityScale = 0f;  // Turn off gravity.
					rb.drag = 0f;  // Change the value of the drag.
					float angle = Abs(Mathf.Atan(verticalMove / horizontalMove));
					rb.AddForce (new Vector2 (Mathf.Cos(angle) *horizontalMove *airDodgeDist,
								Mathf.Sin(angle) *verticalMove *airDodgeDist), ForceMode2D.Impulse);
					rb.drag = airDodgeDrag;
				}

			}
			
			// Cancle everything when total velocity is too low.
			float speed = Mathf.Sqrt(Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.y, 2));
			if (dirAirDodge && speed <= 0.5f) {
				rb.gravityScale = gravity;
				rb.drag = normalDrag;
				isAirDodging = false;
			}

			else if (!dirAirDodge && rb.velocity.y <= -3f) {  // Replace velocity check with timer.
				isAirDodging = false;
			}
		}

		else {
			if (horizontalMove != 0) {
				// Ground movement.
				if (isGrounded) {  // If the player is on the ground.
					if (isWalking) {  // Physics when walking.
						rb.velocity = new Vector2(horizontalMove * walkSpeed, rb.velocity.y);
					}

					else {  // Physics when running.
						if (ableToDash) {  // Performs a dash at the start of the run.
							rb.velocity = new Vector2(dashSpeed * horizontalMove, 0f);
							ableToDash = false;
						}
						else {  // will slow down naturaly due to the friction and keep the player at running speed.
							float x =  Mathf.Clamp(Abs(rb.velocity.x), runSpeed, dashSpeed);
							rb.velocity = new Vector2(x * horizontalMove, 0f);
						}
					}
				}
				// Air movement.
				else { 
					// Slowly accelerate when in the air.
					rb.AddForce (new Vector2(horizontalMove * airAccel, rb.velocity.y), ForceMode2D.Force);
					// Speed will not go faster than the value airSpeed variable.
					float x = Mathf.Clamp(rb.velocity.x, -airSpeed, airSpeed);
					rb.velocity = new Vector2(x, rb.velocity.y);
				}
			}

			if (isFastFalling) rb.velocity = new Vector2(rb.velocity.x, fastFallSpeed);  // Apply constant fast-fall speed.
		}
	}


	void Jump (int jumpType) {  // A standard Jump.
		jumpsUsed++;
		isGrounded = false;  // Manually set var to false to prevent random triple jump bug.
		isFastFalling = false;  // Jumping cancles fast-falls
		rb.velocity = new Vector2(airSpeed *horizontalMove, 0f);  // Reset y velocity to have consistent jump height.
		// Select the jump height depending on the type of jumps performed.
		float jumpHeight = new float[] {fullHopHeight, shortHopHeight, doubleJumpHeight}[jumpType];
		rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);  // The actual jump.
	}


	float Abs (float num) {  // Returns the absolute value of a float.
		if (num < 0) num = -num;
		return(num);
	}
}
