﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {
	public Vector3 velocity;
	public List<GameObject> visionLines;
	public Shader lineShader;
	public GameObject directionLine;
	private bool stop = false;

    private GameObject obstacle;

	protected virtual void Wander(){
		if (stop)
			return;
		AvoidWalls ();
		AvoidObstacles ();
		velocity += new Vector3 (Random.Range (-0.2f, 0.2f), 0.0f, Random.Range (-0.2f, 0.2f));
		velocity.Normalize ();
		//transform.Rotate (new Vector3 (0.0f, Random.Range (-2.0f, 2.0f)));
		LineRenderer lr = directionLine.GetComponent<LineRenderer>();
		lr.SetPosition(0, transform.position);
		lr.SetPosition(1, transform.position + (velocity * 5.0f));
		transform.Translate (velocity * 0.08f);
	}

	protected virtual void AvoidObstacles(){
        Vector3 updatePos = transform.position + velocity;
        foreach (var obstacle in GameObject.FindGameObjectsWithTag("obstacle"))
        {
            float obsAvoidanceFactor = 0.2f;
			if ((updatePos - obstacle.transform.position).magnitude < 1.0f) {
				
				if (updatePos.x - obstacle.transform.position.x < 1f) {
					velocity.x += obsAvoidanceFactor;
				}
				if (updatePos.x - obstacle.transform.position.x < -1f) {
					velocity.x -= obsAvoidanceFactor;
				}
				if (updatePos.z - obstacle.transform.position.z < 1f) {
					velocity.z += obsAvoidanceFactor;
				}
				if (updatePos.z - obstacle.transform.position.z < -1f) {
					velocity.z -= obsAvoidanceFactor;
				}
			}
        }

    }

	protected virtual void AvoidWalls(){
		Vector3 updatePos = transform.position + velocity;
		float wallDistance = 4.5f;
		float wallAvoidanceFactor = 0.6f;
		if (updatePos.x > wallDistance) {
			velocity.x -= wallAvoidanceFactor;
		}
		if (updatePos.x < -wallDistance) {
			velocity.x += wallAvoidanceFactor;
		}
		if (updatePos.z > wallDistance) {
			velocity.z -= wallAvoidanceFactor;
		}
		if (updatePos.z < -wallDistance) {
			velocity.z += wallAvoidanceFactor;
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
	protected void Stop(){
		stop = true;
	}
}
