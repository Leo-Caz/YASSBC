﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ground_check : MonoBehaviour {

	public GameObject character;
	private character character_script;
	private Rigidbody2D rb;

	void Awake() {
		character_script = character.GetComponent<character>();
		rb = character.GetComponent<Rigidbody2D>();
	}

	void OnTriggerEnter2D (Collider2D collider) {
		if ((collider.gameObject.CompareTag("Ground") || collider.gameObject.CompareTag("Plateform")) && rb.velocity.y < 0.1f) {
			character_script.isGrounded = true;
		}
	}

	void OnTriggerExit2D (Collider2D collider) {
		if (collider.gameObject.CompareTag("Ground") || collider.gameObject.CompareTag("Plateform")) {
			character_script.isGrounded = false;
		}
	}
}
