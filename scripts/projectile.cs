using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MonoBehaviour
{
	public Rigidbody2D rb;

	public float speed = 20f;
	public float launchForceX = 0f;
	public float launchForceY = 20f;

    void Awake()
    {
        rb.velocity = new Vector2(speed, 0f);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
		Enemy enemy = hitInfo.GetComponent<Enemy>();
		if (enemy != null) {
			enemy.KnockBack(launchForceX, launchForceY);
			Destroy(gameObject);
		}
    }
}
