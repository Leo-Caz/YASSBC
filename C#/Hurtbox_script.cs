﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox_script : MonoBehaviour {

	public GameObject character;
	private Character_script character_script;

	void Awake() {
		character_script = character.GetComponent<Character_script>();
	}

	void OnTriggerEnter2D (Collider2D collider) {
		if (collider.gameObject.CompareTag("Plateform")) character_script.insidePlateform = true;
	}
	
	void OnTriggerExit2D (Collider2D collider) {
		if (collider.gameObject.CompareTag("Plateform")) character_script.insidePlateform = false;
	}
}