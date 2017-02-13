using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emitter : MonoBehaviour {

	public static List<Ball> Balls;
	public int BallCount;
	public static int currentBallCount;
	// Use this for initialization
	void Start () {
		Balls = new List<Ball> ();
		currentBallCount = 0;	
	}
	
	// Update is called once per frame
	void Update () {
		AddBalls ();
		UpdateBalls ();

	}


	private void AddBalls(){
		int randomFrame = Random.Range (1, 100);
		if (currentBallCount < BallCount && (Time.frameCount % randomFrame) ==0 ) {
			Balls.Add (new Ball ());
			currentBallCount++;
		}
	}
	private void UpdateBalls(){
		foreach (var ball in Balls) {
			ball.Update ();
		}
		for (int i = 0; i < currentBallCount; i++) {
			if (Balls [i].setDestroy) {
				Balls [i].RemoveObj ();
				Balls.RemoveAt (i);
				currentBallCount--;
				i--;
			}
		}
	}
	public class Ball{
		public GameObject obj;
		public Vector3 velocity;
		public bool setDestroy;
		public Ball(){
			//Vector3 randomStart = new Vector3(Random.Range (-5.0f, 5.0f),.5f,Random.Range (-5.0f, 5.0f));
			Vector3 randomStart = new Vector3(Random.Range (-2.5f, 2.5f),30.0f,Random.Range (-2.5f, 2.5f));
			obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			obj.GetComponent<MeshRenderer>().material.color = Random.ColorHSV();
			obj.transform.position = randomStart;
			Vector3 randomStartVelocity = new Vector3(Random.Range (-.5f, .5f),Random.Range (-0.5f, 0.1f),Random.Range (-0.1f, 1.0f));
			velocity = randomStartVelocity;
			setDestroy = false;
			//velocity = new Vector3(0.1f,0.1f,0.1f);
		}
		public void Update(){
			velocity = velocity - new Vector3 (0.0f, 0.01f, 0.0f); //gravity
			if (velocity.magnitude < 0.1f) {
				setDestroy = true;
			}
			CheckFloorCollision();
			CheckWallCollision ();
			obj.transform.Translate (velocity);
			UpdateColor ();
		}
		public void RemoveObj(){
			Destroy (obj);
		}

		private void CheckFloorCollision(){
			float height = obj.transform.position.y + velocity.y;
			if (height < 0.5f) {
				velocity.y = -velocity.y *0.6f; //bounce with dampening
				velocity.x = velocity.x *0.9f;
				velocity.z = velocity.z *0.9f;
			}
		}
		private void UpdateColor(){
			var color = obj.GetComponent<MeshRenderer> ().material.color;
			color.r = color.r * 1.01f;
			color.g = color.g * 0.99f;
			color.b = color.b * 0.99f;
			obj.GetComponent<MeshRenderer> ().material.color = color;
			if(color.r > 1.0f){
				setDestroy = true;
			}
		}
		private void CheckWallCollision(){
			Vector3 updatePos = new Vector3 (obj.transform.position.x + velocity.x, obj.transform.position.y + velocity.y, obj.transform.position.z + velocity.z);

			if ((updatePos.x + updatePos.y) < -15.0f ) {
				float angle = Mathf.Atan2 (velocity.y, velocity.x);
				angle += Mathf.PI / 4.0f;
				velocity.x = Mathf.Sin (angle);
				velocity.y = Mathf.Cos (angle);
			}
			if ((updatePos.x - updatePos.y) > 15.0f ) {
			//	float angle = Mathf.Atan2 (velocity.y, velocity.x);
				velocity.x = -velocity.x;
			}
			if ((updatePos.z - updatePos.y) > 10.0f ) {
			//	float angle = Mathf.Atan2 (velocity.y, velocity.x);
				velocity.z = -velocity.z;
			}
//			if ((updatePos.z + updatePos.y) < -10.0f ) {
//				float angle = Mathf.Atan2 (velocity.y, velocity.x);
//				velocity.z = -velocity.z;
//			}
		}
	}
}
