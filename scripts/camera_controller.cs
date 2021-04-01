using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class camera_controller : MonoBehaviour
{
	public Camera camera;
	public GameObject Player1;
	public GameObject Player2;

	public float scale_factor = 0.23f;
	public float base_scale = 1.75f;
	public float offset = 1.4f;
	public float camera_limit_right  =  18f;
	public float camera_limit_left   = -18f;
	public float camera_limit_top    =  15f;
	public float camera_limit_bottom = -5f;

    void Update() {
		// find the point between the two players
		float x1 = Player1.transform.position.x;
		float y1 = Player1.transform.position.y;
		float x2 = Player2.transform.position.x;
		float y2 = Player2.transform.position.y;
		
		// zoom the camera so that both players are in the frame.
		Vector2 size1 = Player1.GetComponent<BoxCollider2D>().size;
		Vector2 size2 = Player2.GetComponent<BoxCollider2D>().size;
		float dist_x = System.Math.Abs(x2 - x1) + size1.x + size2.x;
		float dist_y = System.Math.Abs(y2 - y1) + size1.y + size2.y;
		float dist_max = dist_x >= dist_y ? dist_x : dist_y;
		camera.orthographicSize = (float)(dist_max * scale_factor) + base_scale;

		// place the camera with a basic offset and prevent it from going out of bounds.
		float camera_x = Mathf.Clamp((((x1 + x2) / 2) / offset),
				camera_limit_left  + (camera.orthographicSize * (16/9)),
				camera_limit_right - (camera.orthographicSize * (16/9)));
		float camera_y = Mathf.Clamp((((y1 + y2) / 2) / offset),
				camera_limit_bottom + camera.orthographicSize,
				camera_limit_top - camera.orthographicSize);
		transform.position = new Vector3(camera_x, camera_y, -10f);
    }
}
