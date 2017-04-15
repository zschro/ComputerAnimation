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
		balls = this.GetComponents<BallGenerator>()[0].Balls;
		balls [0].transform.position = new Vector3(8.0f,1.0f,8.0f);
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

		if (state == State.PickUp) {
			MoveEndEffectorTo (balls [currentBall].transform.position);
			if ((endEffector.transform.position - balls [currentBall].transform.position).magnitude < 0.1f)
				state = State.Drop;

		} else if (state == State.Drop) {
			MoveEndEffectorTo (dropOffSpot);
			if ((endEffector.transform.position - balls [currentBall].transform.position).magnitude < 0.1f) {
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
		RotateArm2 (goalPosition);
		RotateArm1 (goalPosition);
		RotateGrabber (goalPosition);
		LogLinkRotations ();

	}

	private void RotateBase (Vector3 goal){
		var rootToGoal = goal - this.transform.position;
		this.transform.forward = rootToGoal.normalized;
	}

	private void RotateArm1 (Vector3 goal){
		var rootToGoal = goal - arm1.transform.position;
		var rootToEnd = endEffector.transform.position - arm1.transform.position;
		var dot = Vector3.Dot (rootToGoal.normalized, rootToEnd.normalized);
		var angle = Mathf.Acos(dot* Mathf.Deg2Rad)*  Mathf.Rad2Deg;
		Debug.Log ($"Arm1 Angle: {angle}");

		arm1.transform.localEulerAngles = new Vector3 (angle, 0f, 0f);

//		var baseToGoal = goal - this.transform.position;
//		baseToGoal.y = 0;
//		var distanceToGoal = baseToGoal.magnitude;
//
//		Vector2 goal2 = new Vector2(distanceToGoal,0.5f);
//		var root = new Vector2 (arm1.transform.position.z, arm1.transform.position.y);
//		var endEffector2 = new Vector2 (endEffector.transform.position.z, endEffector.transform.position.y);
//
//		var rootToGoal = goal2 - root;
//		var rootToEnd = endEffector2 - root;
//
//		var dot = Vector3.Dot (rootToGoal, rootToEnd);
//		Debug.Log (dot);
//		var angle = Mathf.Acos(dot* Mathf.Deg2Rad) *  Mathf.Rad2Deg;
//		Debug.Log ($"Arm1 Angle: {angle}");
//		arm1.transform.localEulerAngles = new Vector3 (angle, 0f, 0f);
	}

	private void RotateArm2 (Vector3 goal){
		var rootToGoal = goal - arm2.transform.position;
		var rootToEnd = endEffector.transform.position - arm2.transform.position;
		var dot = Vector3.Dot (rootToGoal.normalized, rootToEnd.normalized);
		var angle = Mathf.Acos(dot* Mathf.Deg2Rad)*  Mathf.Rad2Deg;
		Debug.Log ($"Arm2 Angle: {angle}");
		arm2.transform.localEulerAngles = new Vector3 (-angle, 0f, 0f);

//		pe = [xdata(num_of_link+1); ydata(num_of_link+1)];
//		pc = [xdata(iteration-1); ydata(iteration-1)];
//
//		a = (pe - pc)/norm(pe-pc);
//		b = (pt - pc)/norm(pt-pc);
//		teta = acosd(dot(a, b));
	}

	private void RotateGrabber (Vector3 goal){
//		var rootToGoal = goal - claw.transform.position;
//		var rootToEnd = endEffector.transform.position - claw.transform.position;
//		rootToEnd.x = rootToGoal.x = 0;
//
//
//		var dot = Vector3.Dot (rootToGoal, rootToEnd);
//		var anglex = Mathf.Acos(dot* Mathf.Deg2Rad);
//
//		rootToGoal = goal - claw.transform.position;
//		rootToEnd = endEffector.transform.position - claw.transform.position;
//		rootToEnd.y = rootToGoal.y = 0;
//		var doty = Vector3.Dot (rootToGoal, rootToEnd);
//		var angley = Mathf.Acos(doty* Mathf.Deg2Rad);
//
//		var rot = Quaternion.Euler (new Vector3 (anglex, angley, 0f));
//		claw.transform.rotation = rot;
	}

	void LogLinkRotations(){
		Debug.Log ($"End Effector Position: {endEffector.transform.position}");
		Debug.Log ($"Base: {this.transform.localRotation.eulerAngles}");
		Debug.Log ($"Arm1: {arm1.transform.localRotation.eulerAngles}");
		Debug.Log ($"Arm2: {arm2.transform.localRotation.eulerAngles}");
		Debug.Log ($"Claw: {claw.transform.localRotation.eulerAngles}");
	}

}
