using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_script : MonoBehaviour {

	private Rigidbody2D rb;  // rb is used to enable and control the physics of the character.

	public float runSpeed = 8.5f;  // Movement speed when running (not the initial dash)
	private float horizontalMove = 0f;	 // How far analog stick is pressed on horizontal axis (between -1 and +1).

	// Use this for initialization
	void Awake () {
		rb = gameObject.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		horizontalMove = Input.GetAxisRaw("Horizontal");
		rb.velocity = new Vector2(horizontalMove * runSpeed, rb.velocity.y);
	}
}
