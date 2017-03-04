using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prey : MonoBehaviour {
	private Vector3 velocity;
	private List<GameObject> obstacles = new List<GameObject>();
	private List<GameObject> walls = new List<GameObject>();
	private float visionLimit = 0.5f;
	// Use this for initialization
	void Start () {
		velocity = new Vector3 (0.0f, 0.0f, 0.04f);
		LoadObstacles ();
	}
	
	// Update is called once per frame
	void Update () {
		CheckWallCollision ();
		Move();
	}

	private void Move(){
		transform.Translate (velocity);
	}
	private void LoadObstacles(){
		obstacles.AddRange(GameObject.FindGameObjectsWithTag ("obstacle"));
		walls.AddRange(GameObject.FindGameObjectsWithTag ("wall"));
	}
	private void CheckWallCollision(){
		Vector3 updatePos = new Vector3 (transform.position.x + velocity.x, transform.position.y + velocity.y,
			transform.position.z + velocity.z);
		if (updatePos.x > 5.0f) {
			if (velocity.x > velocity.z) {
				transform.Rotate (new Vector3 (0.0f, 120, 0.0f));
			} else {
				transform.Rotate (new Vector3 (0.0f, -120, 0.0f));
			}
		}
		if (updatePos.x < -5.0f) {
			if (velocity.x > velocity.z) {
				transform.Rotate (new Vector3 (0.0f, -120, 0.0f));
			} else {
				transform.Rotate (new Vector3 (0.0f, 120, 0.0f));
			}
		}
		if (updatePos.z > 5.0f) {
			if (velocity.x > velocity.z) {
				transform.Rotate (new Vector3 (0.0f, 120, 0.0f));
			} else {
				transform.Rotate (new Vector3 (0.0f, -120, 0.0f));
			}
		}
		if (updatePos.z < -5.0f) {
			if (velocity.x > velocity.z) {
				transform.Rotate (new Vector3 (0.0f, -120, 0.0f));
			} else {
				transform.Rotate (new Vector3 (0.0f, 120, 0.0f));
			}
		}
	}
}
