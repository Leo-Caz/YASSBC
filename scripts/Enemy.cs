using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	public Rigidbody2D rb;

	void Update() {
		if (gameObject.transform.position.y <= -10) {
			gameObject.transform.position = new Vector2(0f, -1.5f);
		}
	}

    public void KnockBack(float launchForceX, float launchForceY)
    {
        rb.velocity = new Vector2(0f, 0f);
		rb.AddForce(new Vector2(launchForceX, launchForceY), ForceMode2D.Impulse);
    }
}
