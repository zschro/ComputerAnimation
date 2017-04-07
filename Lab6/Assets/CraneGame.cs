using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneGame : MonoBehaviour {

    int numberOfCubes = 5;
    int cubeMinZ = 1;
    int cubeMaxZ = 10;
    int cubeMinX = 1;
    int cubeMaxX = 10;

    Vector3[] cubeLocations;

    void GenerateCubes()
    {
        cubeLocations = new Vector3[numberOfCubes];

        for (int i = 0; i < numberOfCubes; i++)
        {
            cubeLocations[i] = new Vector3(Random.Range(cubeMinX, cubeMaxX), .5f, Random.Range(cubeMinZ, cubeMaxZ));
        }


        for (int i = 0; i < numberOfCubes; i++)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //cube.transform.localScale -= new Vector3(3f, 3f, 3f);
            cube.transform.position = cubeLocations[i];
        }
    }


    // Use this for initialization
    void Start () {
        GenerateCubes();


	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
