using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kinematics : MonoBehaviour {
	Vector3 dropOffSpot = new Vector3 (-8.0f, 1.0f, 8.0f);
	GameObject[] balls;
	int currentBall = 0;

	float prevAngle;
	float nextAngle;
	float dt = 0.1f;
	float t = 0.0f;

	Vector3 prevForward;

	protected enum State{
		PickUp,
		Drop,
		Reset
	}
	State state;

	protected enum MovementState{
		MovingBase,
		MovingClaw,
		MovingArm1,
		MovingArm2,
		Stopped
	}
	MovementState movementState;

	Vector3 goal;
	int tries;

	GameObject arm1;
	GameObject arm2;
	GameObject claw;
	GameObject endEffector;

	// Use this for initialization
	void Start () {
		balls = this.GetComponents<BallGenerator>()[0].GenerateBalls();

		this.state = State.PickUp;
		this.movementState = MovementState.Stopped;
		tries = 0;

		this.arm1 = GameObject.Find ("crane_arm_1");
		this.arm2 = GameObject.Find ("crane_arm_2");
		this.claw = GameObject.Find ("claw");
		this.endEffector = GameObject.Find ("end_effector");

		LogLinkRotations ();
	}

	// Update is called once per frame
	void Update () {

		if (state == State.PickUp) {
			goal = balls [currentBall].transform.position;
			if ((endEffector.transform.position - balls [currentBall].transform.position).magnitude < 0.2f) {
				Debug.Log ("Reached ball");
				tries = 0;
				this.movementState = MovementState.Stopped;
				state = State.Drop;
			} 
			else if (tries < 16) {
				InterpolateMovement ();
			} else {
				Debug.Log ("Skipping ball");
				this.movementState = MovementState.Stopped;
				currentBall++;
				tries = 0;
				if (currentBall >= balls.Length)
					state = State.Reset;
				goal = balls [currentBall].transform.position;
			}
		} else if (state == State.Drop) {
			goal = dropOffSpot;
			InterpolateMovement ();
			if ((endEffector.transform.position - dropOffSpot).magnitude < 0.2f) {
				state = State.PickUp;
				this.movementState = MovementState.Stopped;
				currentBall++;
			}
			if (currentBall >= balls.Length)
				state = State.Reset;
		} else if (state == State.Reset) {
			foreach (var ball in balls) {
				Destroy (ball);
			}
			balls = this.GetComponents<BallGenerator>()[0].GenerateBalls();
			currentBall = 0;

			state = State.PickUp;
			this.movementState = MovementState.Stopped;
			tries = 0;
		}
	}

	private void InterpolateMovement(){
		t += dt;
		float angle = prevAngle + t * (nextAngle - prevAngle);

		switch (movementState) {
		case(MovementState.Stopped):
			prevForward = this.transform.forward;
			movementState = MovementState.MovingBase;
			break;
		case(MovementState.MovingClaw):
			claw.transform.localEulerAngles = new Vector3 (angle, 0f, 0f);
			if (t >= 1.0f) {
				t = 0;
				movementState = MovementState.MovingArm2;
				prevAngle = arm2.transform.localEulerAngles.x;
				nextAngle = GetLinkAngle (goal, arm2, prevAngle);
			}
			break;
		case(MovementState.MovingArm1):
			arm1.transform.localEulerAngles = new Vector3 (angle, 0f, 0f);
			if (t >= 1.0f) {
				t = 0;
				tries++;
				movementState = MovementState.MovingArm2;
				prevAngle = arm2.transform.localEulerAngles.x;
				nextAngle = GetLinkAngle (goal, arm2, prevAngle);
//				movementState = MovementState.MovingClaw;
//				prevAngle = claw.transform.localEulerAngles.x;
//				var clawAngle = GetLinkAngle (goal, claw, prevAngle);
//				nextAngle = clawAngle > 120.0f ? 120.0f : clawAngle;
			}
			break;
		case(MovementState.MovingArm2):
			arm2.transform.localEulerAngles = new Vector3 (angle, 0f, 0f);
			if (t >= 1.0f) {
				t = 0;
				movementState = MovementState.MovingArm1;
				prevAngle = arm1.transform.localEulerAngles.x;
				nextAngle = GetLinkAngle (goal, arm1, prevAngle);
			}
			break;
		case(MovementState.MovingBase):
			var end = goal - this.transform.position;
			this.transform.forward = prevForward + t * (end - prevForward);
			if (t >= 1.0f) {
				t = 0;
				movementState = MovementState.MovingArm2;
				prevAngle = arm2.transform.localEulerAngles.x;
				nextAngle = GetLinkAngle (goal, arm2, prevAngle);
//				movementState = MovementState.MovingClaw;
//				prevAngle = claw.transform.localEulerAngles.x;
//				nextAngle = GetLinkAngle (goal, arm1, prevAngle);
			}
			break;
		default:
			break;
		}
	}
		
	private float GetLinkAngle(Vector3 goal, GameObject link, float currentAngle){
		var rootToGoal = goal - link.transform.position;
		var rootToEnd = endEffector.transform.position - link.transform.position;
		var dot = Vector3.Dot (rootToGoal.normalized, rootToEnd.normalized);
		var angle = Mathf.Acos(dot)*  Mathf.Rad2Deg;
		Debug.Log ($"New Angle: {angle}, Previous: {currentAngle}, MovementState: {movementState}, Goal: {goal}");

		var cross = Vector3.Cross (rootToGoal, rootToEnd);
		//get the cross product to find the direction of rotation
		if (cross.x > 0.0f)
			return currentAngle - angle;
		else
			return currentAngle + angle;
	}


	void LogLinkRotations(){
		Debug.Log ($"End Effector Position: {endEffector.transform.position}");
		Debug.Log ($"Base: {this.transform.localRotation.eulerAngles}");
		Debug.Log ($"Arm1: {arm1.transform.localRotation.eulerAngles}");
		Debug.Log ($"Arm2: {arm2.transform.localRotation.eulerAngles}");
		Debug.Log ($"Claw: {claw.transform.localRotation.eulerAngles}");
	}

}
