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
			//canSeePrey = FindPrey();
		} else {
			//ChasePrey();
		}
	}
	private void FindPrey(){

	}
}
