﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {
	public Vector3 velocity;
	public List<GameObject> visionLines;
	public Shader lineShader;
	public GameObject directionLine;

	protected virtual void Wander(){
		AvoidWalls ();
		AvoidObstacles ();
		velocity += new Vector3 (Random.Range (-0.2f, 0.2f), 0.0f, Random.Range (-0.2f, 0.2f));
		velocity.Normalize ();
		//transform.Rotate (new Vector3 (0.0f, Random.Range (-2.0f, 2.0f)));
		transform.Translate (velocity * 0.1f);
		LineRenderer lr = directionLine.GetComponent<LineRenderer>();
		lr.SetPosition(0, transform.position);
		lr.SetPosition(1, transform.position + (velocity * 5.0f));
	}

	protected virtual void AvoidObstacles(){

	}

	protected virtual void AvoidWalls(){
		Vector3 updatePos = transform.position + velocity;
		float wallDistance = 19.5f;
		if (updatePos.x > wallDistance || updatePos.x < -wallDistance) {
			velocity.x = -velocity.x;
		}
		if (updatePos.z > wallDistance || updatePos.z < -wallDistance) {
			velocity.z = -velocity.z;
		}
	}
		
	protected void SetupVisionLines(Color color){
		directionLine = new GameObject();
		directionLine.transform.parent = transform;
		directionLine.AddComponent<LineRenderer>();
		LineRenderer lr = directionLine.GetComponent<LineRenderer>();
		//lr.material = new Material(lineShader);
		lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
		color.a = 0.5f;
		lr.startColor = color;
		lr.endColor = color;
		lr.startWidth = 0.1f;
		lr.endWidth = 5.0f;
	}

	//	private void LoadObstacles(){
	//		obstacles.AddRange(GameObject.FindGameObjectsWithTag ("obstacle"));
	//		walls.AddRange(GameObject.FindGameObjectsWithTag ("wall"));
	//	}
}