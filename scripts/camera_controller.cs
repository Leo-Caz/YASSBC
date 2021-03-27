using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Camera_controller : MonoBehaviour
{
	public Camera camera;
	public GameObject Player1;
	public GameObject Player2;

    void Update() {
		// placer la caméra entre les deux joueurs.
		float x1 = Player1.transform.position.x;
		float y1 = Player1.transform.position.y;
		float x2 = Player2.transform.position.x;
		float y2 = Player2.transform.position.y;
		transform.position = new Vector3((x1 + x2) / 2, (y1 + y2) / 2, -10f);

		// zoom camera pour laisser les deux joueurs visibles.
		Vector2 size1 = Player1.GetComponent<BoxCollider2D>().size;
		Vector2 size2 = Player2.GetComponent<BoxCollider2D>().size;
		float dist_x = System.Math.Abs(x2 - x1) + size1.x + size2.x;
		float dist_y = System.Math.Abs(y2 - y1) + size1.y + size2.y;
		float dist_max = dist_x >= dist_y ? dist_x : dist_y;
		camera.orthographicSize = (float)(dist_max * 0.2) + 3;
    }
}
