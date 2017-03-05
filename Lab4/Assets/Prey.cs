using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prey : Agent {
	
	private bool canSeePredator;
	// Use this for initialization
	void Start () {
		velocity = new Vector3 (0.0f, 0.0f, 0.04f);
		canSeePredator = false;
		SetupVisionLines (Color.blue);
	}
	
	// Update is called once per frame
	void Update () {
		
		if (!canSeePredator) {
			Wander ();
			//canSeePredator = FindPredator();
		} else {
			//RunAway();
		}

	}
}
