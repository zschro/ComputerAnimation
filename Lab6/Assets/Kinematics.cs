using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kinematics : MonoBehaviour {
	Vector3 dropOffSpot = new Vector3 (-8.0f, 3.0f, 8.0f);
	List<GameObject> balls;
	int currentBallIndex = 0;

	float[] prevLinkageAngles;
	float[] nextLinkageAngles;
	float dt = 0.025f;
	float t = 0.0f;

	Vector3 prevForward;

	protected enum State{
		PickUp,
		Drop,
		BallFalling,
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
	GameObject currentBall;
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
		currentBall = balls [0];
		balls.RemoveAt(0);
		tries = 0;

		arm1 = GameObject.Find ("crane_arm_1");
		arm2 = GameObject.Find ("crane_arm_2");
		claw = GameObject.Find ("claw");
		endEffector = GameObject.Find ("end_effector");
		prevLinkageAngles = new float[3]{0,0,0};
		nextLinkageAngles = new float[3]{0,0,0};

		LogLinkRotations ();
	}

	// Update is called once per frame
	void Update () {
		Material mat;
		if (state == State.PickUp) {
			goal = currentBall.transform.position;
			mat = currentBall.GetComponent<MeshRenderer> ().material;
			mat.color = Color.green;

			if ((endEffector.transform.position - currentBall.transform.position).magnitude < 0.4f) {
				Debug.Log ("Reached ball");
				tries = 0;
				this.movementState = MovementState.Stopped;
				state = State.Drop;
			} 
			else if (tries < 5) {
				InterpolateMovement ();
			} else {
				Debug.Log ("Skipping ball");
				this.movementState = MovementState.Stopped;
				mat = currentBall.GetComponent<MeshRenderer> ().material;
				mat.color = Color.red;
				if (balls.Count > 0) {
					currentBall = balls [0];
					balls.RemoveAt (0);
				}
				else
					state = State.Reset;
				tries = 0;
				goal = currentBall.transform.position;
			}
		} else if (state == State.Drop) {
			goal = dropOffSpot;
			InterpolateMovement ();
			currentBall.transform.position = endEffector.transform.position;
			if ((endEffector.transform.position - dropOffSpot).magnitude < 1.0f) {
				state = State.BallFalling;
			}
		} else if (state == State.Reset) {
			balls.AddRange (this.GetComponents<BallGenerator> () [0].GenerateBalls ());
			currentBall = balls [0];
			balls.RemoveAt(0);

			state = State.PickUp;
			this.movementState = MovementState.Stopped;
			tries = 0;
		}
		else if (state == State.BallFalling) {
			var ballPos = currentBall.transform.position;
			if (ballPos.y > 0.5f) {
				currentBall.transform.position = new Vector3 (ballPos.x, ballPos.y - .25f, ballPos.z);
			} else {
				state = State.PickUp;
				this.movementState = MovementState.Stopped;
				if (balls.Count > 0) {
					currentBall = balls [0];
					balls.RemoveAt (0);
				}
				else
					state = State.Reset;
				dropOffSpot += (Random.insideUnitSphere *2.0f);
			}
			
		}
        userPickBall();

    }

	private void InterpolateMovement(){
		float angle;

		switch (movementState) {
		case(MovementState.Stopped):
			prevForward = this.transform.forward;
			movementState = MovementState.MovingBase;
			t = 0;
			break;
		case(MovementState.MovingBase):
			var end = goal - this.transform.position;
			end.y = 0;
			this.transform.forward = prevForward + t * (end - prevForward);
			if (t > 1.0f) {
				t = 0;
				movementState = MovementState.MovingClaw;
				//prevLinkageAngles [0] = claw.transform.localRotation.eulerAngles.x;
				var clawAngle = GetLinkAngle (goal, claw, prevLinkageAngles[0]);
				if (clawAngle > 90.0f)
					clawAngle = 90.0f;
				if (clawAngle < -10.0f)
					clawAngle = -10.0f;
				
				nextLinkageAngles[0] = clawAngle;
			}
			break;
		case(MovementState.MovingClaw):
			angle = nextLinkageAngles[0] + t * (nextLinkageAngles [0] - prevLinkageAngles[0]);
			claw.transform.localRotation = Quaternion.Euler (angle, 0f, 0f);
			if (t >= 1.0f) {
				t = 0;
				prevLinkageAngles [0] = nextLinkageAngles [0];
				movementState = MovementState.MovingArm2;
				//prevLinkageAngles [1] = arm2.transform.localRotation.eulerAngles.x;
				nextLinkageAngles [1] = GetLinkAngle (goal, arm2, prevLinkageAngles [1]);
				var arm2Angle = GetLinkAngle (goal, arm2, prevLinkageAngles [1]);
				if (arm2Angle > 130.0f)
					arm2Angle = 130.0f;
				if (arm2Angle < -10.0f)
					arm2Angle = -10.0f;
				nextLinkageAngles [1] = arm2Angle;

			}
			break;
		case(MovementState.MovingArm2):
			angle = prevLinkageAngles [1] + t * (nextLinkageAngles [1] - prevLinkageAngles [1]);
			arm2.transform.localRotation = Quaternion.Euler (angle, 0f, 0f);
			if (t >= 1.0f) {
				t = 0;
				prevLinkageAngles [1] = nextLinkageAngles [1];
				movementState = MovementState.MovingArm1;
				var arm1Angle = GetLinkAngle (goal, arm1, prevLinkageAngles [2]);
				if (arm1Angle > 95.0f)
					arm1Angle = 95.0f;
				if (arm1Angle < -10.0f)
					arm1Angle = -10.0f;
				//prevLinkageAngles [2] = arm1.transform.localRotation.eulerAngles.x;
				nextLinkageAngles[2] = arm1Angle;
			}
			break;
		case(MovementState.MovingArm1):
			angle = prevLinkageAngles[2] + t * (nextLinkageAngles[2] - prevLinkageAngles[2]);
			arm1.transform.localRotation = Quaternion.Euler (angle, 0f, 0f);
			if (t >= 1.0f) {
				t = 0;
				prevLinkageAngles[2] = nextLinkageAngles [2];
				tries++;
				movementState = MovementState.MovingClaw;
				//prevLinkageAngles [0] = claw.transform.localRotation.eulerAngles.x;
				var clawAngle = GetLinkAngle (goal, claw, prevLinkageAngles[0]);
				if (clawAngle > 90.0f)
					clawAngle = 90.0f;
				if (clawAngle < -10.0f)
					clawAngle = -10.0f;
				nextLinkageAngles[0] = clawAngle;
			}
			break;
		default:
			break;
		}
		t += dt;
	}
		
	private float GetLinkAngle(Vector3 goal, GameObject link, float currentAngle){
		var rootToGoal = goal - link.transform.position;
		var rootToEnd = endEffector.transform.position - link.transform.position;
		var dot = Vector3.Dot (rootToGoal.normalized, rootToEnd.normalized);
		var angle = Mathf.Acos(dot)*  Mathf.Rad2Deg;
		Debug.Log ($"New Angle: {angle}, Previous: {currentAngle}, MovementState: {movementState}, Goal: {goal}, link: {link.name}");

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

    private void userPickBall()
    {
        GameObject clickedBall;
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.name == "Sphere") {
                    clickedBall = hit.collider.gameObject;
					var mat = clickedBall.GetComponent<MeshRenderer> ().material;
					mat.color = Color.magenta;

					movementState = MovementState.Stopped;
					state = State.PickUp;
					balls.Remove (clickedBall);
					currentBall.transform.position = new Vector3(currentBall.transform.position.x,0.5f,currentBall.transform.position.z);
					mat = currentBall.GetComponent<MeshRenderer> ().material;
					mat.color = Color.red;

					currentBall = clickedBall;
                }
            }

            
        }
        
    }

}
