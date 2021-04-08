using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class melee : MonoBehaviour
{
	public BoxCollider2D bc;
	public float launchForceX = 0f;
	public float launchForceY = 20f;

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
		Enemy enemy = hitInfo.GetComponent<Enemy>();
		if (enemy != null) {
			enemy.KnockBack(launchForceX, launchForceY);
			bc.enabled = false;
		}
    }
}
