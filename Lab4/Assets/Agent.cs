using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {
	public Vector3 velocity;
	public List<GameObject> visionLines;
	public Shader lineShader;
    private GameObject obstacle;
	private int numberOfVisionLines = 6;
	protected State state;
	protected enum State{
		RunAway,
		Wander,
		AvoidWalls,
		AvoidObstacles,
		ChasePrey
	}

	protected virtual void Wander(){
		velocity += new Vector3 (Random.Range (-0.2f, 0.2f), 0.0f, Random.Range (-0.2f, 0.2f));
		AvoidWalls ();
		AvoidObstacles ();
	}
	protected virtual void Move(){
		velocity.Normalize ();
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
		float wallDistance = 24.5f;
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
		for (int i = 0; i < numberOfVisionLines; i++) {
			var directionLine = new GameObject();
			directionLine.transform.parent = transform;
			directionLine.AddComponent<LineRenderer>();
			LineRenderer lr = directionLine.GetComponent<LineRenderer>();
			lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
			color.a = 0.80f;
			lr.startColor = color;
			lr.endColor = color;
			lr.startWidth = 0.1f;
			lr.endWidth = 0.30f;
			visionLines.Add (directionLine);
		}

	}
}
