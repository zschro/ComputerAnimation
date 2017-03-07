using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prey : Agent {
	private GameObject predatorToAvoid;
	private RaycastHit obstacleToAvoid;
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
		if (this.state == State.RunAway) {
			RunAway ();
		} else {
			Wander ();
		}
		Move ();
	}

	private void RunAway(){
		velocity.Normalize ();
		Vector3 agentToVertex = transform.position - predatorToAvoid.transform.position ;
		velocity += (agentToVertex.normalized * 0.3f);
		sprintCount--;
		if (sprintCount < 1) {
			this.state = State.Wander;
			Debug.Log ("Prey Changed State - Wander");
			var mat = this.GetComponent<MeshRenderer> ().material;
			mat.color = Color.blue;
		}
	}
    private void RaycastVision()
    {
		int i = -3;
		foreach (var visionLine in visionLines) {
			Vector3 rayCast = Quaternion.Euler(0, i*10, 0) * velocity;
			RaycastHit hit;
			LineRenderer lr = visionLine.GetComponent<LineRenderer>();
			lr.SetPosition(0, transform.position);

			if (Physics.Raycast(transform.position, rayCast, out hit, 7.0f))
			{
				if(hit.collider != null)
				{
					if (hit.collider.gameObject.tag == "predator") {
						predatorToAvoid = hit.collider.gameObject;
						this.state = State.RunAway;
						sprintCount = 300;
						Debug.Log ("Prey Changed State - Run Away");
						var mat = this.GetComponent<MeshRenderer> ().material;
						mat.color = Color.cyan;
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
				lr.SetPosition(1, transform.position + rayCast.normalized * 7.0f);
			}
			i++;
		}
    }

}
