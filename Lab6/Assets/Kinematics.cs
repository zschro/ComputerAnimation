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
		
		this.transform.Rotate (new Vector3 (0f, 1.0f, 0f));
		LogLinkRotations ();

	}

	void LogLinkRotations(){
		Debug.Log ($"End Effector Position: {endEffector.transform.position}");
		Debug.Log ($"Base: {this.transform.localRotation.eulerAngles}");
		Debug.Log ($"Arm1: {arm1.transform.localRotation.eulerAngles}");
		Debug.Log ($"Arm2: {arm2.transform.localRotation.eulerAngles}");
		Debug.Log ($"Claw: {claw.transform.localRotation.eulerAngles}");
	}

}
