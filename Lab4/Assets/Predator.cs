using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Predator : Agent {
	private GameObject preyTarget;
	private RaycastHit obstacleToAvoid;


	void Start () {
		velocity = new Vector3 (0.0f, 0.0f, 0.0f);
		this.state = State.Wander;
		preyTarget = null;
		SetupVisionLines (Color.red);
	}

	void Update () {		
		RaycastVision ();
		if (this.state == State.ChasePrey) {
			ChasePrey ();
		} else {
			Wander ();
		}
		Move ();
	}

	private void RaycastVision()
	{
		int i = -3;
		foreach (var visionLine in visionLines) {
			Vector3 rayCast = Quaternion.Euler(0, i*10, 0) * velocity;
			RaycastHit hit;
			LineRenderer lr = visionLine.GetComponent<LineRenderer>();
			lr.SetPosition(0, transform.position);

			if (Physics.Raycast(transform.position, rayCast, out hit, 5.0f))
			{
				if(hit.collider != null)
				{
					if (hit.collider.tag == "prey") {
						preyTarget = hit.collider.gameObject;
						this.state = State.ChasePrey;
						Debug.Log ("Predator Changed State - Chase");
						var mat = this.GetComponent<MeshRenderer> ().material;
						mat.color = Color.magenta;
					}
				}
				lr.SetPosition(1, hit.point);
				lr.startColor = Color.green;
				lr.endColor = Color.green;   
			}
			else
			{
				lr.startColor = Color.red;
				lr.endColor = Color.red;
				lr.SetPosition(1, transform.position + rayCast.normalized * 5.0f);
			}
			i++;
		}
	}
		
	private void ChasePrey(){
		if (preyTarget == null) {
			this.state = State.Wander;
			Debug.Log ("Predator Changed State - Wander");
			return;
		}
		Vector3 agentToVertex = transform.position - preyTarget.transform.position ;
		velocity -= (agentToVertex.normalized * 1.5f);
		AvoidWalls ();
		AvoidObstacles ();
		if (agentToVertex.magnitude < 1.0f) {
			preyTarget.tag = "dead";
			Destroy (preyTarget);
			preyTarget = null;
			this.state = State.Wander;
			Debug.Log ("Predator Changed State - Wander");
		}
		if (agentToVertex.magnitude > 6.0f) {
			preyTarget = null;
			this.state = State.Wander;
			Debug.Log ("Predator Changed State - Wander");
		}
	}
}
