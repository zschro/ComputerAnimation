using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallGenerator : MonoBehaviour {

    int numberOfCubes = 5;
    int cubeMinZ = 2;
    int cubeMaxZ = 14;
    int cubeMinX = -3;
    int cubeMaxX = 12;

    Vector3[] cubeLocations;

	public GameObject[] GenerateBalls()
    {
		GameObject[] Balls= new GameObject[5];
        cubeLocations = new Vector3[numberOfCubes];
        for (int i = 0; i < numberOfCubes; i++)
        {
            cubeLocations[i] = new Vector3(Random.Range(cubeMinX, cubeMaxX), 2.5f, Random.Range(cubeMinZ, cubeMaxZ));
        }
        for (int i = 0; i < numberOfCubes; i++)
        {
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			var mat = cube.GetComponent<MeshRenderer> ().material;
			mat.color = Color.red;
            cube.transform.position = cubeLocations[i];
			Balls [i] = cube;
        }
		return Balls;
    }
}
