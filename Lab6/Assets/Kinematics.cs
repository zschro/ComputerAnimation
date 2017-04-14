using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kinematics : MonoBehaviour {
	float craneBaseHeight = 1.5f;
	float arm1Length = 6.0f;
	float arm2Length = 6.0f;
	float grabberLength = 0.75f;
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
		balls = this.GetComponents<BallGenerator>()[0].Balls;
		this.state = State.PickUp;

		this.arm1 = GameObject.Find ("crane_arm_1");
		this.arm2 = GameObject.Find ("crane_arm_2");
		this.claw = GameObject.Find ("claw");
		this.endEffector = GameObject.Find ("end_effector");
		LogLinkRotations ();
	}
	
	// Update is called once per frame
	void Update () {

		if (state == State.PickUp) {
			MoveEndEffectorTo (balls [currentBall].transform.position);
		} else if (state == State.Drop) {
			MoveEndEffectorTo (new Vector3(-8.0f,2.0f,8.0f));//drop off position above walled in area
		}


	}

	void MoveEndEffectorTo(Vector3 goalPosition){

		RotateBase (goalPosition);
		//RotateArm1 (goalPosition);
		RotateArm2 (goalPosition);
		RotateGrabber (goalPosition);
		//LogLinkRotations ();

	}

	private void RotateBase (Vector3 goal){
		var rootToGoal = goal - this.transform.position;
		var rootToEnd = endEffector.transform.position - this.transform.position;
		rootToEnd.y = rootToGoal.y = 0;
		Debug.Log ($"RootToGoal: {rootToGoal}");
		Debug.Log ($"RootToEnd: {rootToEnd}");
		var dot = Vector3.Dot (rootToGoal, rootToEnd);
		var angle = Mathf.Acos(dot* Mathf.Deg2Rad);
		Debug.Log ($"Dot: {dot}");
		Debug.Log ($"Angle: {angle}");
		if(angle > .001f)
			this.transform.Rotate (new Vector3 (0f,angle , 0f));
	}

	private void RotateArm1 (Vector3 goal){
		var rootToGoal = goal - arm1.transform.position;
		var rootToEnd = endEffector.transform.position - arm1.transform.position;
		rootToEnd.x = rootToGoal.x = 0;
		Debug.Log ($"RootToGoal: {rootToGoal}");
		Debug.Log ($"RootToEnd: {rootToEnd}");
		var dot = Vector3.Dot (rootToGoal, rootToEnd);
		var angle = Mathf.Acos(dot* Mathf.Deg2Rad);
		Debug.Log ($"Dot: {dot}");
		Debug.Log ($"Angle: {angle}");
		if(angle > .001f)
			arm1.transform.Rotate (new Vector3 (angle, 0f, 0f));
	}

	private void RotateArm2 (Vector3 goal){
		var rootToGoal = goal - arm2.transform.position;
		var rootToEnd = endEffector.transform.position - arm2.transform.position;
		rootToEnd.x = rootToGoal.x = 0;
		Debug.Log ($"RootToGoal: {rootToGoal}");
		Debug.Log ($"RootToEnd: {rootToEnd}");
		var dot = Vector3.Dot (rootToGoal, rootToEnd);
		var angle = Mathf.Acos(dot* Mathf.Deg2Rad);
		Debug.Log ($"Dot: {dot}");
		Debug.Log ($"Angle: {angle}");
		if(angle > .001f)
			arm2.transform.Rotate (new Vector3 (angle, 0f, 0f));
	}

	private void RotateGrabber (Vector3 goal){
		var rootToEnd = endEffector.transform.position - claw.transform.position;
		claw.transform.Rotate (rootToEnd);
	}

	private void CoordinateDescent(Vector3 endEffectorPos, Vector3 goalPosition, Transform linkTransform){
		var rootToGoal = goalPosition - linkTransform.position;
		var rootToEnd = endEffectorPos - linkTransform.position;
		var angle = Mathf.Acos(Vector3.Dot (rootToGoal, rootToEnd));

	}

	void LogLinkRotations(){
		Debug.Log ($"End Effector Position: {endEffector.transform.position}");
		Debug.Log ($"Base: {this.transform.localRotation.eulerAngles}");
		Debug.Log ($"Arm1: {arm1.transform.localRotation.eulerAngles}");
		Debug.Log ($"Arm2: {arm2.transform.localRotation.eulerAngles}");
		Debug.Log ($"Claw: {claw.transform.localRotation.eulerAngles}");
	}

}
