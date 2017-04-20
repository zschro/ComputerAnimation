using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallGenerator : MonoBehaviour {

    int numberOfCubes = 5;
    float minZ = 3;
	float maxZ = 14;
	float minX = -2;
	float maxX = 12;

	public List<GameObject> GenerateBalls()
    {
		List<GameObject> Balls= new List<GameObject>();
        for (int i = 0; i < numberOfCubes; i++)
        {
			GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			var mat = ball.GetComponent<MeshRenderer> ().material;
			mat.color = Color.blue;
			ball.transform.position = new Vector3(Random.Range(minX, maxX), 0.5f, Random.Range(minZ, maxZ));
			Balls.Add (ball);
        }
		return Balls;
    }
}
