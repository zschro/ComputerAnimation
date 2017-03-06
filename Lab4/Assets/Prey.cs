using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prey : Agent {
	
	private bool canSeePredator;
	private GameObject predatorToAvoid;
	private int sprintCount = 0;
	// Use this for initialization
	void Start () {
		velocity = new Vector3 (0.0f, 0.0f, 0.00f);
		canSeePredator = false;
		SetupVisionLines (Color.blue);
	}
	
	// Update is called once per frame
	void Update () {
		
		if (!canSeePredator) {
			Wander ();
			FindPredator();
		} else {
			RunAway();
		}

	}
	private void FindPredator(){
		Vector3 orientation = velocity;
		foreach (var predator in GameObject.FindGameObjectsWithTag ("predator")) {
			Vector3 agentToVertex = predator.transform.position - transform.position;
			if (agentToVertex.magnitude < 6.0f) {
				//prey is within range
				agentToVertex.Normalize ();
				//Debug.Log ($"check: {Vector3.Dot (agentToVertex, orientation)}, {prey.name}");
				if (Mathf.Abs(Vector3.Dot (agentToVertex, orientation)) > 0.8f) {
					predatorToAvoid = predator;
					Debug.Log ($"prey saw predator: {Vector3.Dot (agentToVertex, orientation)}, {predator.name}");
					canSeePredator = true;
					sprintCount += 100;
					return;
				}
			}
		}
		canSeePredator = false;
	}

	private void RunAway(){
		AvoidWalls ();
		AvoidObstacles ();
		Vector3 agentToVertex = transform.position - predatorToAvoid.transform.position ;
		velocity += (agentToVertex.normalized * 0.1f);
		velocity.Normalize ();
		//transform.Rotate (new Vector3 (0.0f, Random.Range (-2.0f, 2.0f)));
		LineRenderer lr = directionLine.GetComponent<LineRenderer>();
		lr.SetPosition(0, transform.position);
		lr.SetPosition(1, transform.position + (velocity * 5.0f));
		transform.Translate (velocity * 0.2f);
		sprintCount--;
		if(sprintCount < 1)
			canSeePredator = false;
	}
}
