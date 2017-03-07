using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prey : Agent {
	private RaycastHit predatorToAvoid;
	private int sprintCount = 0;

	// Use this for initialization
	void Start () {
		velocity = new Vector3 (0.0f, 0.0f, 0.0f);
		state = State.Wander;
		SetupVisionLines (Color.blue);
		var mat = this.GetComponent<MeshRenderer> ().material;
		mat.color = Color.blue;
	}
	
	// Update is called once per frame
	void Update () {
        RaycastVision();
		if (this.state == State.AvoidWalls) {
			AvoidWalls ();
		} else if (this.state == State.AvoidObstacles) {
			AvoidObstacles ();
		} else if (this.state == State.RunAway) {
			RunAway ();
		} else {
			Wander ();
		}
		Move ();
	}

	private void RunAway(){
		AvoidWalls ();
		AvoidObstacles ();
		Vector3 agentToVertex = transform.position - predatorToAvoid.transform.position ;
		velocity += (agentToVertex.normalized * 0.1f);
		velocity.Normalize ();
		transform.Translate (velocity * 0.2f);
		sprintCount--;
		if (sprintCount < 1) {
			this.state = State.Wander;
			var mat = this.GetComponent<MeshRenderer> ().material;
			mat.color = Color.blue;
		}
	}
    private void RaycastVision()
    {
		int i = -3;
		foreach (var visionLine in visionLines) {
			Vector3 rayCast = Quaternion.Euler(0, 0, i*10) * velocity;
			RaycastHit hit;
			LineRenderer lr = visionLine.GetComponent<LineRenderer>();
			lr.SetPosition(0, transform.position);

			if (Physics.Raycast(transform.position, rayCast, out hit, 5.0f))
			{
				if(hit.collider != null)
				{
					Debug.Log("something hit: " + hit.collider);
					if (hit.collider.tag == "wall") {
						this.state = State.AvoidWalls;
					} else if (hit.collider.tag == "obstacle") {
						this.state = State.AvoidObstacles;
					} else if (hit.collider.tag == "predator") {
						predatorToAvoid = hit;
						this.state = State.RunAway;
					} else {
						this.state = State.Wander;
					}
				}
				lr.SetPosition(1, hit.point);
				lr.startColor = Color.green;
				lr.endColor = Color.green;   
			}
			else
			{
				lr.startColor = Color.blue;
				lr.endColor = Color.blue;
				lr.SetPosition(1, transform.position + rayCast.normalized * 5.0f);
				Wander();
			}
			i++;
		}
    }

}
