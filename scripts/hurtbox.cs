using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hurtbox : MonoBehaviour {

	public GameObject character;
	private character character_script;

	void Awake() {
		character_script = character.GetComponent<character>();
	}

	void OnTriggerEnter2D (Collider2D collider) {
		if (collider.gameObject.CompareTag("Plateform")) character_script.insidePlateform = true;
	}
	
	void OnTriggerExit2D (Collider2D collider) {
		if (collider.gameObject.CompareTag("Plateform")) character_script.insidePlateform = false;
	}
}
