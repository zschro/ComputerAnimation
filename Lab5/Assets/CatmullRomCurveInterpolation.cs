using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatmullRomCurveInterpolation : MonoBehaviour {
	
	const int NumberOfPoints = 8;
	Vector3[] controlPoints;
    Vector3[] subPoints;
	List<Vector3> travelPoints;
	int nextTravelPoint = 0;

    double[] arclengths = new double[NumberOfPoints];
    float totalLength;

	const int MinX = -5;
	const int MinY = -5;
	const int MinZ = 0;

	const int MaxX = 5;
	const int MaxY = 5;
	const int MaxZ = 5;
	
	float time = 0;
	const float DT = 0.005f;
	private Matrix4x4 catmulRom;
	
	/* Returns a point on a cubic Catmull-Rom/Blended Parabolas curve
	 * u is a scalar value from 0 to 1
	 * segment_number indicates which 4 points to use for interpolation
	 */
	Vector3 ComputePointOnCatmullRomCurve(float u, int segmentNumber)
	{
		Vector3 point = new Vector3();
		int p0 = segmentNumber;

		int p_minus_2 = (segmentNumber + NumberOfPoints - 2) % (NumberOfPoints);
		int p_minus_1 = (segmentNumber + NumberOfPoints - 1) % (NumberOfPoints);
		int p_1 = (segmentNumber + 1) % (NumberOfPoints);


		//Debug.Log ($"p0: {p0}, p_1: {p_1}, p_-2: {p_minus_2}");

		Vector4 uVector = new Vector4 (u * u * u, u * u, u, 1);
		var xVector = new Vector4(controlPoints[p_minus_2].x,controlPoints[p_minus_1].x,
			controlPoints[p0].x,controlPoints[p_1].x);
		var yVector = new Vector4(controlPoints[p_minus_2].y,controlPoints[p_minus_1].y,
			controlPoints[p0].y,controlPoints[p_1].y);
		var zVector = new Vector4(controlPoints[p_minus_2].z,controlPoints[p_minus_1].z,
			controlPoints[p0].z,controlPoints[p_1].z);
		
		var xResult = catmulRom * xVector;
		point.x = Vector4.Dot(uVector, xResult);

		var yResult = catmulRom * yVector;
		point.y = Vector4.Dot(uVector, yResult);

		var zResult = catmulRom * zVector;
		point.z = Vector4.Dot(uVector, zResult);

		return point;
	}
	
	void GenerateControlPointGeometry()
	{
		for(int i = 0; i < NumberOfPoints; i++)
		{
			GameObject tempcube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			tempcube.transform.localScale -= new Vector3(0.8f,0.8f,0.8f);
			tempcube.transform.position = controlPoints[i];
		}	
	}
	
	// Use this for initialization
	void Start () {

		controlPoints = new Vector3[NumberOfPoints];

		// set points randomly...
		controlPoints[0] = new Vector3(0,0,0);

        
        for (int i = 1; i < NumberOfPoints; i++)
		{
			controlPoints[i] = new Vector3(Random.Range(MinX,MaxX),Random.Range(MinY,MaxY),Random.Range(MinZ,MaxZ));
        }

        GenerateControlPointGeometry();

		catmulRom.SetRow (0, new Vector4 (-0.5f, 1.5f, -1.5f, 0.5f));
		catmulRom.SetRow (1, new Vector4 (1.0f, -2.5f, 2.0f, -0.5f));
		catmulRom.SetRow (2, new Vector4 (-0.5f, 0f, 0.5f, 0f));
		catmulRom.SetRow (3, new Vector4 (0f, 1.0f, 0f, 0f));
		ReparameterizeArcLength ();
	}

	private void ReparameterizeArcLength(){
		float clength = 0;
		subPoints = new Vector3[NumberOfPoints * 10000];
		travelPoints = new List<Vector3>();
		travelPoints.Add(ComputePointOnCatmullRomCurve(0,0));
		float subsectionLength = 0;

		Vector3 dpoint;
		for (int i = 0; i < NumberOfPoints; i++)
		{
			for (int j = 0; j < 10000; j++)
			{
				//next point
				//Vector3 furtherPoint = controlPoints[i] * (.1f * j);
				Vector3 furtherPoint = ComputePointOnCatmullRomCurve(j * 0.0001f,i);
				subPoints[(i * 10000) + j] = furtherPoint;
				//calculate distance between new point and old point
				if(i + j == 0)
				{
					dpoint = new Vector3 ();
					//dpoint = subPoints[0] - furtherPoint;
				}
				else
				{
					dpoint = subPoints[(i * 10000) + j - 1] - furtherPoint;
				}
				subsectionLength += dpoint.magnitude;
				if (subsectionLength > .03f) {
					travelPoints.Add (furtherPoint);
					subsectionLength = 0.0f;
				}
				//calculate arclength, add to running total and running total arraylist
				clength += dpoint.magnitude;
				arclengths[i] = clength;
			}
		}

		totalLength += clength;
		Debug.Log(totalLength);
	}

	// Update is called once per frame
	void Update () {
		int nextPoint = nextTravelPoint++ % travelPoints.Count;
		Vector3 temp = travelPoints [nextPoint];
		transform.forward = temp - transform.position;
		Debug.Log ($"Speed:{(temp - transform.position).magnitude}");
		transform.position = temp;

//		time += DT;
//			
//		// TODO - use time to determine values for u and segment_number in this function call
//		int segmentNumber = Mathf.RoundToInt(time - .5f);
//		if (segmentNumber > (NumberOfPoints - 1)) {
//			segmentNumber--;	
//			time -= NumberOfPoints;
//		}
//		float u = (time - segmentNumber);
//
//		
//		Vector3 temp = ComputePointOnCatmullRomCurve(u,segmentNumber);
//		transform.forward = temp - transform.position;

//		transform.position = temp;
	}
}
