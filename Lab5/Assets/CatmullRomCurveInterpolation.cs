using UnityEngine;
using System.Collections;

public class CatmullRomCurveInterpolation : MonoBehaviour {
	
	const int NumberOfPoints = 8;
	Vector3[] controlPoints;
	
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


		Debug.Log ($"p0: {p0}, p_1: {p_1}, p_-2: {p_minus_2}");

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
		for(int i = 1; i < NumberOfPoints; i++)
		{
			controlPoints[i] = new Vector3(Random.Range(MinX,MaxX),Random.Range(MinY,MaxY),Random.Range(MinZ,MaxZ));
		}
		/*...or hard code them for testing
		controlPoints[0] = new Vector3(0,0,0);
		controlPoints[1] = new Vector3(0,0,0);
		controlPoints[2] = new Vector3(0,0,0);
		controlPoints[3] = new Vector3(0,0,0);
		controlPoints[4] = new Vector3(0,0,0);
		controlPoints[5] = new Vector3(0,0,0);
		controlPoints[6] = new Vector3(0,0,0);
		controlPoints[7] = new Vector3(0,0,0);
		*/
		
		GenerateControlPointGeometry();
		catmulRom.SetRow (0, new Vector4 (-0.5f, 1.5f, -1.5f, 0.5f));
		catmulRom.SetRow (1, new Vector4 (1.0f, -2.5f, 2.0f, -0.5f));
		catmulRom.SetRow (2, new Vector4 (-0.5f, 0f, 0.5f, 0f));
		catmulRom.SetRow (3, new Vector4 (0f, 1.0f, 0f, 0f));
	}
	
	// Update is called once per frame
	void Update () {
		
		time += DT;
			
		// TODO - use time to determine values for u and segment_number in this function call
		int segmentNumber = Mathf.RoundToInt(time - .5f);
		if (segmentNumber > (NumberOfPoints - 1)) {
			segmentNumber--;	
			time -= NumberOfPoints;
		}
		float u = (time - segmentNumber);
		Debug.Log ($"U: {u}, SegmentNumber: {segmentNumber}, Time: {time}");
		
		Vector3 temp = ComputePointOnCatmullRomCurve(u,segmentNumber);
		transform.forward = transform.position - temp;
		transform.position = temp;

	}
}
