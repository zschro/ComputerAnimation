using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunter : MonoBehaviour {
	private float v;
	private List<GameObject> obstacles = new List<GameObject>();
	private Vector3 orientationVector;
	private float visionLimit = 0.1f;

	void Start () {
		v = 0.02f;
		orientationVector = new Vector3 (0.0f, 0.0f, 1.0f);
		LoadObstacles ();
	}

	void Update () {
		//Find Prey
		Steer();
		Move();
	}
	private void Steer(){
		
		foreach (var obstacle in obstacles) {
			var agentToObstacle = obstacle.transform.position - this.transform.position;
			agentToObstacle.Normalize ();
			float agentToObstacleScalar = Vector3.Dot (agentToObstacle, orientationVector);
			if (agentToObstacleScalar < visionLimit) {
				if (Vector3.Magnitude (transform.position - obstacle.transform.position) < 1.5f) {
					var leftTurn = orientationVector + new Vector3 (.05f, 0.0f, 0.0f);
					var rightTurn = orientationVector - new Vector3 (.05f, 0.0f, 0.0f);
					if (Vector3.Dot (agentToObstacle, leftTurn) < Vector3.Dot (agentToObstacle, rightTurn)) {
						orientationVector += new Vector3 (0.005f, 0.0f, 0.0f);
						transform.rotation *= Quaternion.LookRotation (orientationVector);
					} else {
						orientationVector -= new Vector3 (0.005f, 0.0f, 0.0f);
						transform.rotation *= Quaternion.LookRotation (orientationVector);
					}
				}
			}
		}

	}
	private void Move(){
		transform.Translate(new Vector3(0.0f,0.0f,v),Space.Self);
	}
	private void LoadObstacles(){
		obstacles.AddRange(GameObject.FindGameObjectsWithTag ("obstacle"));
		obstacles.AddRange(GameObject.FindGameObjectsWithTag ("wall"));
	}
}
