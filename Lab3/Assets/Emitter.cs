using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emitter : MonoBehaviour {

	public static List<Ball> Balls;
	public int BallCount;
	public static int currentBallCount;
    public static float startVelocity = 0.1f;
    public static int avgBallAddSpeed = 5;
	public static Vector3 start = new Vector3 (0f, 30f, 0f);

	private static readonly Vector3 leftWallNormal = new Vector3 (0.7f, 0.7f);
	private static readonly Vector3 rightWallNormal = new Vector3 (-0.7f, 0.7f);
	private static readonly Vector3 rearWallNormal = new Vector3 (0.0f,0.7f, -0.7f);

	private static readonly float dampeningEffect = 1.0f;

	// Use this for initialization
	void Start () {
		Balls = new List<Ball> ();
		currentBallCount = 0;
	}
	
	// Update is called once per frame
	void Update () {
		AddBalls ();
		UpdateBalls ();
		MoveStart ();
		UpdateParams ();
    }

	private void AddBalls(){
		for (int i = 0; i < 3; i++) {
			int randomFrame = Random.Range (1, avgBallAddSpeed);
			if (currentBallCount < BallCount && (Time.frameCount % randomFrame) ==0 ) {
				Balls.Add (new Ball ());
				currentBallCount++;
			}
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
	private void UpdateParams(){
		if (Input.GetKey(KeyCode.W))
		{
			startVelocity += .01f;
		}
		if (Input.GetKey(KeyCode.S))
		{
			if(startVelocity > .01f)
			{
				startVelocity -= .01f;
			}    
		}
		if (Input.GetKey(KeyCode.A))
		{
			avgBallAddSpeed++;
		}
		if (Input.GetKey(KeyCode.D))
		{
			if (avgBallAddSpeed > 1)
			{
				avgBallAddSpeed--;
			}
		}
	}
	private void MoveStart(){
		if (Input.GetKey(KeyCode.UpArrow)){
			start.z +=0.2f;
		}
		if (Input.GetKey(KeyCode.LeftArrow)){
			start.x -=0.2f;
		}
		if (Input.GetKey(KeyCode.RightArrow)){
			start.x +=0.2f;
		}
		if (Input.GetKey(KeyCode.DownArrow)){
			start.z -=0.2f;
		}
		var capsule = GameObject.Find ("Capsule");
		capsule.transform.position = start;
	}

	public class Ball{
		public GameObject obj;
		public Vector3 velocity;
		public bool setDestroy;
		public Ball(){
			//Vector3 randomStart = new Vector3(Random.Range (-5.0f, 5.0f),.5f,Random.Range (-5.0f, 5.0f));
			Vector3 randomStart = new Vector3(Random.Range (-2.5f, 2.5f),0f,Random.Range (-2.5f, 2.5f));
			randomStart += start;
			obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			obj.GetComponent<MeshRenderer>().material.color = new Color(Random.Range(0.0f,0.5f),0.5f,0.5f);
			obj.transform.position = randomStart;
			Vector3 randomStartVelocity = new Vector3(Random.Range (-startVelocity, startVelocity),Random.Range (-startVelocity, startVelocity),Random.Range (-startVelocity, startVelocity + 0.50f));
			velocity = randomStartVelocity;
			setDestroy = false;
			//velocity = new Vector3(0.1f,0.1f,0.1f);
		}
		public void Update(){
			velocity = velocity - new Vector3 (0.0f, 0.015f, 0.0f); //gravity
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
				velocity.y = -velocity.y;
				velocity = velocity * dampeningEffect;
			}

		}
		private void UpdateColor(){
			var color = obj.GetComponent<MeshRenderer> ().material.color;
			color.r = color.r * 1.001f;
			color.g = color.g * 0.99f;
			color.b = color.b * 0.99f;
			obj.GetComponent<MeshRenderer> ().material.color = color;
			if(color.r > 1.1f && color.g < .2f && color.b < .2f){
				setDestroy = true;
			}
		}
		private void CheckWallCollision(){
			Vector3 updatePos = new Vector3 (obj.transform.position.x + velocity.x, obj.transform.position.y + velocity.y, obj.transform.position.z + velocity.z);
			// E(p) = ax + by + cz + d = N . p + d
			// E(p) < 0 collision
			bool hit = false;
			if ((updatePos.x + updatePos.y) < -15.0f ) {
				var wallNormal = leftWallNormal;
				var u = (Vector3.Dot (velocity, wallNormal) / Vector3.Dot (wallNormal, wallNormal)) * wallNormal ;
				var w = velocity - u;
				velocity = w - u;
				hit = true;
			}
			if ((updatePos.x - updatePos.y) > 15.0f ) {
				var wallNormal = rightWallNormal;
				var u = (Vector3.Dot (velocity, wallNormal) / Vector3.Dot (wallNormal, wallNormal)) * wallNormal ;
				var w = velocity - u ;
				velocity = w - u;
				hit = true;
			}
			if ((updatePos.z - updatePos.y) > 10.0f ) {
				var wallNormal = rearWallNormal;
				var u = (Vector3.Dot (velocity, wallNormal) / Vector3.Dot (wallNormal, wallNormal)) * wallNormal ;
				var w = velocity - u;
				velocity = w - u;
				hit = true;
			}
			if (hit) {
				velocity = velocity * dampeningEffect; //slight dampening for wall hit
			}
		}

	}
}
