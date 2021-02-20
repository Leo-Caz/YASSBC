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
	private bool isWalking = false;       // Determines if normal atk button will do smashs or tilts, and if player will walk or run.
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
		horizontalMove = Input.GetAxisRaw("Horizontal");  // Gets position of analog stick on x-axis
		verticalMove = Input.GetAxisRaw("Vertical");  // Gets position of analog stick on x-axis

		// When smash mode is activated, you walk instead of dash, and do smash attacks instead of tilts.
		if (Input.GetKeyDown(KeyCode.LeftShift)) isWalking = true;
		else if (Input.GetKeyUp(KeyCode.LeftShift)) isWalking = false;

		// Calls the Jump function when spacebar is pressed and you still have jumps left.
		if ((Input.GetKeyDown(KeyCode.Space)) && (jumpsUsed < maxNbJumps)) {
			if (isGrounded && !isWalking) Jump(0);      // Perform a standard jump.
			else if (isGrounded && isWalking) Jump(1);  // Perform a short jump.
			else if (!isGrounded) Jump(2);              // Perform a double jump.
		}

		if (Input.GetKeyDown(KeyCode.N) && !isGrounded) isAirDodging = true;

		if (Input.GetKeyDown(KeyCode.X)) Physics2D.IgnoreLayerCollision(10, 9);  // Go through platforms.
		else if (!insidePlateform) {
			Physics2D.IgnoreLayerCollision(10, 9, false);  // Reactivate collisions when out of platform.
		}
		
		if (rb.velocity.y < 0.5f && Input.GetKeyDown(KeyCode.C)) isFastFalling = true;  // Perform a fast-fall.
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
			if (horizontalMove != 0) {  // If the player is moving.
				if (isGrounded) {  // If the player is on the ground.
					if (isWalking) {  // Physics when walking.
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
			
			if (isFastFalling) rb.velocity = new Vector2(rb.velocity.x, fastFallSpeed);  // Apply fast-fall speed.
			else if (rb.velocity.y < 0) {  // Limits the maximum speed of the player when falling down.
				float y =  Mathf.Clamp(rb.velocity.y, maxFallSpeed, 0f);
				rb.velocity = new Vector2(rb.velocity.x, y);
			}
		}
	}


	void Jump (int jumpType) {  // A standard Jump.
		jumpsUsed++;
		isGrounded = false;  // Manually set var to false to prevent random triple jump bug.
		isFastFalling = false;  // Jumping cancles fast-falls
		rb.velocity = new Vector2(rb.velocity.x, 0f);  // Reset y velocity to have consistent jump height.
		// Select the jump height depending on the type of jumps performed.
		float jumpHeight = new float[] {fullHopHeight, shortHopHeight, doubleJumpHeight}[jumpType];
		rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);  // The actual jump.
	}


	float Abs (float num) {  // Returns the absolute value of a float.
		if (num < 0) num = -num;
		return(num);
	}
}
using system.collections.generic;
using unityengine;

public class character_script : monobehaviour {

	private rigidbody2d rb;  // rb is used to enable and control the physics of the character.

	public float dashspeed = 16f;         // speed of the initial dash.
	public float runspeed  = 9.5f;        // movement speed when running (not the initial dash)
	public float walkspeed = 4.5f;        // maximum movement speed when walking.
	public float fullhopheight = 25f;     // height of the normal jump.
	public float shorthopheight = 18f;    // height of the short jump.
	public float doublejumpheight = 23f;  // height of the double jump.
	public int maxnbjumps = 2;            // number of times the player can jump before touching the ground.
	public float maxfallspeed = -8.5f;    // maximum fall speed without fast falling.
	public float fastfallspeed = -14.5f;  // speed of the player when performing a fast-fall.
	public float airaccel = 14.5f;        // acceleration when moving in the air.
	public float airspeed = 7.75f;        // maximum speed in the air.
	public float normaldrag = 2.6f;       // value of the regular linear drag.
	public float airdodgedrag = 6.5f;     // value of the linear drag during an air dodge.
	public float airdodgedist = 11f;      // distance of the air dodge.
	public float gravity = 4.3f;          // gravity of the character.

	public bool isgrounded = false;       // determines if the player is touching the ground.
	public bool insideplateform = false;  // determines if the player is inside a platform when going through.
	public bool isfastfalling = false;    // determines if the player performes a fast-fall or not.
	public bool isairdodging = false;    // determines if the player is in air dodge state.

	private float horizontalmove = 0f;	  // position of the analog stick x-axis (between -1 and +1).
	private float verticalmove = 0f;	  // position of the analog stick y-axis (between -1 and +1).
	private bool abletodash = true;       // determines if the player does the initial dash when running.
	private bool iswalking = false;       // determines if normal atk button will do smashs or tilts, and if player will walk or run.
	private int jumpsused = 0;            // number of jumps performed before touching the ground.
	private bool abletoairdodge = true;   // determines if the player is able to perform an air dodge.
	private bool dirairdodge = false;     // determines if the player performed a directional ad or a normal one.


	// use this for initialization
	void awake () {
		rb = gameobject.getcomponent<rigidbody2d>();
	}
	

	// update is called once per frame
	void update () {
		horizontalmove = input.getaxisraw("horizontal");  // gets position of analog stick on x-axis
		verticalmove = input.getaxisraw("vertical");  // gets position of analog stick on x-axis

		// when smash mode is activated, you walk instead of dash, and do smash attacks instead of tilts.
		if (input.getkeydown(keycode.leftshift)) iswalking = true;
		else if (input.getkeyup(keycode.leftshift)) iswalking = false;

		// calls the jump function when spacebar is pressed and you still have jumps left.
		if ((input.getkeydown(keycode.space)) && (jumpsused < maxnbjumps)) {
			if (isgrounded && !iswalking) jump(0);      // perform a standard jump.
			else if (isgrounded && iswalking) jump(1);  // perform a short jump.
			else if (!isgrounded) jump(2);              // perform a double jump.
		}

		if (input.getkeydown(keycode.n) && !isgrounded) isairdodging = true;
		if (input.getkeydown(keycode.c) && isgrounded) {  // macro for a wavedash to the right.
			isairdodging = true;
			horizontalmove = 1f;
			verticalmove = -1f;
		}
		if (input.getkeydown(keycode.z) && isgrounded) {  // macro for a wave dash to the left.
			isairdodging = true;
			horizontalmove = -1f;
			verticalmove = -1f;
		}

		if (input.getkeydown(keycode.x)) physics2d.ignorelayercollision(10, 9);  // go through platforms.
		else if (!insideplateform) {
			physics2d.ignorelayercollision(10, 9, false);  // reactivate collisions when out of platform.
		}
		
		if (rb.velocity.y < 0.5f && input.getkeydown(keycode.c)) isfastfalling = true;  // perform a fast-fall.
	}


	void fixedupdate() {
		// when performing an air dodge.
		if (isairdodging) {
			if (abletoairdodge) {
				abletoairdodge = false;
				// deactivate hurbox
				if (abs(horizontalmove) >= 0.1f || abs(verticalmove) >= 0.1f) {
					rb.velocity = new vector2(0f, 0f);
					dirairdodge = true;
					rb.gravityscale = 0f;  // turn off gravity.
					rb.drag = 0f;  // change the value of the drag.
					float angle = abs(mathf.atan(verticalmove / horizontalmove));
					rb.addforce (new vector2 (mathf.cos(angle) *horizontalmove *airdodgedist,
								mathf.sin(angle) *verticalmove *airdodgedist), forcemode2d.impulse);
					rb.drag = airdodgedrag;
				}
			}
			
			// cancle everything when total velocity is too low.
			float speed = mathf.sqrt(mathf.pow(rb.velocity.x, 2) + mathf.pow(rb.velocity.y, 2));
			if (dirairdodge && speed <= 0.3f) {
				rb.gravityscale = gravity;
				rb.drag = normaldrag;
				isairdodging = false;
			}

			else if (!dirairdodge && rb.velocity.y <= -3f) {  // replace velocity check with timer.
				isairdodging = false;
			}
		}

		else {
			if (horizontalmove != 0) {  // if the player is moving.
				// ground movement.
				if (isgrounded) {  // if the player is on the ground.
					if (iswalking) {  // physics when walking.
						rb.velocity = new vector2(horizontalmove * walkspeed, rb.velocity.y);
					}
					else {  // physics when running.
						if (abletodash) {  // performs a dash at the start of the run.
							rb.addforce (new vector2(dashspeed * horizontalmove, 0f), forcemode2d.impulse);
							abletodash = false;
						}
						else {  // will slow down naturaly due to the friction and keep the player at running speed.
							float x =  mathf.clamp(abs(rb.velocity.x), runspeed, dashspeed);
							rb.velocity = new vector2(x * horizontalmove, 0f);
						}
					}
				}
				// air movement.
				else {  // if the player is in the air.
					// slowly accelerate when in the air.
					rb.addforce (new vector2(horizontalmove * airaccel, rb.velocity.y), forcemode2d.force);
					// speed will not go faster than the value airspeed variable.
					float x = mathf.clamp(rb.velocity.x, -airspeed, airspeed);
					rb.velocity = new vector2(x, rb.velocity.y);
				}
			}
			
			// fast falling.
			if (isfastfalling) rb.velocity = new vector2(rb.velocity.x, fastfallspeed);  // apply fast-fall speed.
			else if (rb.velocity.y < 0) {  // limits the maximum speed of the player when falling down.
				float y =  mathf.clamp(rb.velocity.y, maxfallspeed, 0f);
				rb.velocity = new vector2(rb.velocity.x, y);
			}
		}
		
		// when touching the ground.
		if (isgrounded) {
			jumpsused = 0;  // resets the number of jumps performed when touching the ground.
			isfastfalling = false; // deactivates the fast-fall.
			abletoairdodge = true;
			if (isairdodging) {
				isairdodging = false;  // cancles the air dodge when landing to allow for combos.
				rb.gravityscale = gravity;  // reactivate gravity.
				rb.drag = normaldrag;  // set the linear drag to the regular value.
				rb.velocity = new vector2 ((abs(rb.velocity.x) + abs(rb.velocity.y)) *horizontalmove, 0f);
			}
		}
	}


	void jump (int jumptype) {  // a standard jump.
		jumpsused++;
		isgrounded = false;  // manually set var to false to prevent random triple jump bug.
		isfastfalling = false;  // jumping cancles fast-falls
		rb.velocity = new vector2(rb.velocity.x, 0f);  // reset y velocity to have consistent jump height.
		// select the jump height depending on the type of jumps performed.
		float jumpheight = new float[] {fullhopheight, shorthopheight, doublejumpheight}[jumptype];
		rb.addforce(vector2.up * jumpheight, forcemode2d.impulse);  // the actual jump.
	}


	float abs (float num) {  // returns the absolute value of a float.
		if (num < 0) num = -num;
		return(num);
	}
}
