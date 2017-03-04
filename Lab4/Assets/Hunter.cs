using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunter : MonoBehaviour {
	private Vector3 velocity;
	private List<GameObject> obstacles = new List<GameObject>();
	private List<GameObject> walls = new List<GameObject>();
	private float visionLimit = 0.5f;

	void Start () {
		velocity = new Vector3 (0.0f, 0.0f, 0.06f);
		LoadObstacles ();
	}

	void Update () {
		//Find Prey
		Steer();
		Move();
	}
	private void Steer(){
		CheckWallCollision ();
		foreach (var obstacle in obstacles) {
			Vector3 orientationVector = velocity.normalized;
			var agentToObstacle = obstacle.transform.position - this.transform.position;
			agentToObstacle.Normalize ();
			float agentToObstacleScalar = Vector3.Dot (agentToObstacle, orientationVector);
			if (agentToObstacleScalar < visionLimit) {
				if (Vector3.Magnitude (transform.position - obstacle.transform.position) < 0.5f) {
					var leftTurn = orientationVector + new Vector3 (.05f, 0.0f, 0.0f);
					var rightTurn = orientationVector - new Vector3 (.05f, 0.0f, 0.0f);
					if (Vector3.Dot (agentToObstacle, leftTurn) < Vector3.Dot (agentToObstacle, rightTurn)) {
						transform.Rotate (0.0f, 5.0f, 0.0f);
					} else {
						transform.Rotate (0.0f, -5.0f, 0.0f);
					}
				}
			}
		}

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
