using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground_check : MonoBehaviour {

	public bool isGrounded = true;

	void OnTriggerEnter2D(Collider2D collider) {
	    Debug.Log("Entered");
	    if (collider.gameObject.CompareTag("Ground")) {
	        isGrounded = true;
	    }
	}

	void OnTriggerExit2D(Collider2D collider) {
	    Debug.Log("Exited");
	    if (collider.gameObject.CompareTag("Ground")) {
	        isGrounded = false;
	    }
	}
}
