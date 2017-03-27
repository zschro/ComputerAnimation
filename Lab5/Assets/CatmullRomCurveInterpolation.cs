using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatmullRomCurveInterpolation : MonoBehaviour {
	
	const int NumberOfPoints = 3;
	Vector3[] controlPoints;
	List<Vector3> travelPoints;
	int nextTravelPoint = 0;
	float maxSpeed = 0.04f;
	float minSpeed = 0.001f;
	float acceleration = 0.001f;

	int numberOfSubPoints = 2000;
	Vector3[] subPoints;
	double[] arclengths;

	const int MinX = -5;
	const int MinY = -5;
	const int MinZ = 0;

	const int MaxX = 5;
	const int MaxY = 5;
	const int MaxZ = 5;

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
		arclengths = new double[NumberOfPoints * numberOfSubPoints];
		subPoints = new Vector3[NumberOfPoints * numberOfSubPoints];


		// set points randomly...
		controlPoints[0] = new Vector3(0,0,0);
        for (int i = 1; i < NumberOfPoints; i++)
		{
			controlPoints[i] = new Vector3(Random.Range(MinX,MaxX),Random.Range(MinY,MaxY),Random.Range(MinZ,MaxZ));
        }

        GenerateControlPointGeometry();

		//Hard Coded tau = .5 matrix
		catmulRom.SetRow (0, new Vector4 (-0.5f, 1.5f, -1.5f, 0.5f));
		catmulRom.SetRow (1, new Vector4 (1.0f, -2.5f, 2.0f, -0.5f));
		catmulRom.SetRow (2, new Vector4 (-0.5f, 0f, 0.5f, 0f));
		catmulRom.SetRow (3, new Vector4 (0f, 1.0f, 0f, 0f));

		ReparameterizeArcLength ();
	}

	private void ReparameterizeArcLength(){
		float totalLength = 0;

		double subsectionLength = 0;
		float increment = 1.0f / (float)numberOfSubPoints;
		Vector3 dpoint;

		for (int i = 0; i < NumberOfPoints; i++)
		{
			for (int j = 0; j < numberOfSubPoints; j++)
			{
				//next point
				//Vector3 furtherPoint = controlPoints[i] * (.1f * j);
				Vector3 furtherPoint = ComputePointOnCatmullRomCurve(j * increment,i);
				subPoints[(i * numberOfSubPoints) + j] = furtherPoint;
				//calculate distance between new point and old point
				if(i + j == 0)
				{
					dpoint = new Vector3 ();
				}
				else
				{
					dpoint = subPoints[(i * numberOfSubPoints) + j - 1] - furtherPoint;
				}
				//calculate arclength, add to running total and running total arraylist
				totalLength += dpoint.magnitude;
				arclengths[(i * numberOfSubPoints) + j] = totalLength;
			}
		}
		Debug.Log(totalLength);

		//initialize points that will be used as actual movement points
		travelPoints = new List<Vector3>();
		travelPoints.Add(ComputePointOnCatmullRomCurve(0,0));
		int lastPoint = 0;
		float speed = 0.0f;
		int numberOfSpeedUpPoints = 0;
		Debug.Log ($"subPoint:{subPoints.Length}");
		for (int i = 1; i < subPoints.Length; i++) {
			subsectionLength = arclengths [i] - arclengths [lastPoint];
				
			Debug.Log ($"sectionLength:{subsectionLength}");
			if (subsectionLength > speed) {
				travelPoints.Add (subPoints[i]);
				lastPoint = i;

				if (i < numberOfSubPoints/3) {
					//speed up
					speed += acceleration;
				}
				if (subPoints.Length - i < numberOfSubPoints/3) {
					//slow down
					speed -= acceleration/2;
				}


			}
		}
	}

	// Update is called once per frame
	void Update () {
		int nextPoint = nextTravelPoint++ % travelPoints.Count;
		Vector3 temp = travelPoints [nextPoint];
		transform.forward = temp - transform.position;
		Debug.Log ($"Speed:{(temp - transform.position).magnitude}");
		transform.position = temp;
	}
}
