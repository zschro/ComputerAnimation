using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Predator : Agent {
	private bool canSeePrey;
	private GameObject preyTarget;


	void Start () {
		velocity = new Vector3 (0.0f, 0.0f, 0.06f);
		canSeePrey = false;
		preyTarget = null;
		SetupVisionLines (Color.red);
	}

	void Update () {		
		//Find Prey
		if (!canSeePrey) {
			Wander ();
			FindPrey();
		} else {
			ChasePrey();
		}
	}
	private void FindPrey(){
		Vector3 orientation = velocity;
		foreach (var prey in GameObject.FindGameObjectsWithTag ("prey")) {
			Vector3 agentToVertex = prey.transform.position - transform.position;
			if (agentToVertex.magnitude < 5.0f) {
				//prey is within range
				agentToVertex.Normalize ();
				Debug.Log ($"check: {Vector3.Dot (agentToVertex, orientation)}, {prey.name}");
				if (Mathf.Abs(Vector3.Dot (agentToVertex, orientation)) > 0.9f) {
					preyTarget = prey;
//					prey.SendMessage ("Stop");
//					Stop ();
					Debug.Log ($"hit: {Vector3.Dot (agentToVertex, orientation)}, {prey.name}");
					var mat = prey.GetComponent<MeshRenderer> ().material;
					mat.color = Color.magenta;
					canSeePrey = true;
					return;
				}
			}
		}
		canSeePrey = false;
	}
	private void ChasePrey(){
		Vector3 agentToVertex = transform.position - preyTarget.transform.position ;
		velocity -= agentToVertex.normalized * 0.5f;
		velocity.Normalize ();
		//transform.Rotate (new Vector3 (0.0f, Random.Range (-2.0f, 2.0f)));
		LineRenderer lr = directionLine.GetComponent<LineRenderer>();
		lr.SetPosition(0, transform.position);
		lr.SetPosition(1, transform.position + (velocity * 5.0f));
		transform.Translate (velocity * 0.15f);
		if (agentToVertex.magnitude < 1.0f) {
			Destroy (preyTarget);
			preyTarget = null;
			canSeePrey = false;
		}
	}
}
