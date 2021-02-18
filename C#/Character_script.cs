using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_script : MonoBehaviour {

	private Rigidbody2D rb;  // rb is used to enable and control the physics of the character.

	public float runSpeed = 8.5f;  // Movement speed when running (not the initial dash)
	public float walkSpeed = 3.5f;  // Movement speed when walking.
	public float jumpHeight = 25f;  // Height of the peak of the jump.
	public float maxFallSpeed = 8.5f;  // Maximum fall speed without fast falling.
	private float horizontalMove = 0f;	 // How far analog stick is pressed on horizontal axis (between -1 and +1).
	private bool smashMode = false;  // Determines if the normal attack button will do smashs or tilt attacks,
									 // and if the player will walk or run.

	// Use this for initialization
	void Awake () {
		rb = gameObject.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		horizontalMove = Input.GetAxisRaw("Horizontal");
		if (Input.GetKeyDown(KeyCode.A)) {
			smashMode = true;
		}
		else if (Input.GetKeyUp(KeyCode.A)) {
			smashMode = false;
		}

		if (smashMode) {
			rb.velocity = new Vector2(horizontalMove * runSpeed, rb.velocity.y);
		}
		else {
			rb.velocity = new Vector2(horizontalMove * walkSpeed, rb.velocity.y);
		}
		
		if (Input.GetKeyDown(KeyCode.Space)) {
			rb.velocity = new Vector2(rb.velocity.x, 0f);
			rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
		}

		Debug.Log(rb.velocity.y);
		if (rb.velocity.y < 0) {
			float y =  Mathf.Clamp(rb.velocity.y, maxFallSpeed, 0f);
			rb.velocity = new Vector2(rb.velocity.x, y);
		}
	}
}
