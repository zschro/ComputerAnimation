using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kinematics : MonoBehaviour {
	float craneBaseHeight = 1.5f;
	float arm1Length = 6.0f;
	float arm2Length = 6.0f;
	float grabberLength = 0.75f;
	Vector3 dropOffSpot = new Vector3 (-8.0f, 2.0f, 8.0f);
	GameObject[] balls;
	int currentBall = 0;

	float rotationSpeed;

	protected enum State{
		PickUp,
		Drop,
		Rest
	}
	State state;

	GameObject arm1;
	GameObject arm2;
	GameObject claw;
	GameObject endEffector;

	// Use this for initialization
	void Start () {
		balls = this.GetComponents<BallGenerator>()[0].GenerateBalls();
		Debug.Log (balls);
		Debug.Log (balls.Length);

		balls [0].transform.position = new Vector3(0.0f,1.0f,8.0f);
		var mat = balls [0].GetComponent<MeshRenderer> ().material;
		mat.color = Color.green;

		this.state = State.PickUp;

		this.arm1 = GameObject.Find ("crane_arm_1");
		this.arm2 = GameObject.Find ("crane_arm_2");
		this.claw = GameObject.Find ("claw");
		this.endEffector = GameObject.Find ("end_effector");
		LogLinkRotations ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.frameCount % 10 != 0)
			return;
		Debug.Log ($"State: {state}");
		if (state == State.PickUp) {
			MoveEndEffectorTo (balls [currentBall].transform.position);
			if ((endEffector.transform.position - balls [currentBall].transform.position).magnitude < 0.2f)
				state = State.Drop;

		} else if (state == State.Drop) {
			MoveEndEffectorTo (dropOffSpot);
			if ((endEffector.transform.position - dropOffSpot).magnitude < 0.2f) {
				state = State.PickUp;
				currentBall++;
			}
			if (currentBall >= balls.Length)
				state = State.Rest;
		} else if (state == State.Rest) {
			//do nothing
		}


	}

	void MoveEndEffectorTo(Vector3 goalPosition){

		RotateBase (goalPosition);
		RotateLink (goalPosition, claw);
		RotateLink (goalPosition, arm2);
		RotateLink (goalPosition, arm1);
	}

	private void RotateBase (Vector3 goal){
		var rootToGoal = goal - this.transform.position;
		this.transform.forward = rootToGoal.normalized;
	}

	private void RotateLink(Vector3 goal, GameObject link){
		var rootToGoal = goal - link.transform.position;
		Debug.Log ($"rootToGoal: {rootToGoal}, normalize: {rootToGoal.normalized}");
		var rootToEnd = endEffector.transform.position - link.transform.position;
		Debug.Log ($"rootToEnd: {rootToEnd}, normalize: {rootToEnd.normalized}");
		var dot = Vector3.Dot (rootToGoal.normalized, rootToEnd.normalized);
		Debug.Log ($"dot: {dot}");
		var angle = Mathf.Acos(dot)*  Mathf.Rad2Deg;
		Debug.Log ($"Arm1 Angle: {angle}");

		var cross = Vector3.Cross(rootToGoal,rootToEnd);
		Debug.Log ($"Arm1 cross: {cross}");
		var previousAngle = arm1.transform.localEulerAngles.x;

		if(cross.x> 0.0f)
			link.transform.localEulerAngles = new Vector3 (previousAngle-angle, 0f, 0f);
		else
			link.transform.localEulerAngles = new Vector3 (previousAngle+angle, 0f, 0f);
	}

	void LogLinkRotations(){
		Debug.Log ($"End Effector Position: {endEffector.transform.position}");
		Debug.Log ($"Base: {this.transform.localRotation.eulerAngles}");
		Debug.Log ($"Arm1: {arm1.transform.localRotation.eulerAngles}");
		Debug.Log ($"Arm2: {arm2.transform.localRotation.eulerAngles}");
		Debug.Log ($"Claw: {claw.transform.localRotation.eulerAngles}");
	}

}
