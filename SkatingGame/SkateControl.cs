using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class SkateControl : MonoBehaviour {

	public Camera cam; 
	public float Speed = 1f; 
	public float RotatingSpeed = 1f; 
	public float AdditionalGravity = 0.5f; 

	public bool reverse = false; 

	[HideInInspector] public Rigidbody rb; 
	InputProcessing inputs; 
	float height; 

	public bool aerial;
	// Use this for initialization
	void Start () {

		Initialization(); 

	}
	
	// Update is called once per frame
	void Update () {
		CheckPhysics(); 

		Vector2 direction = inputs.GetDirection();
		SkaterMove(direction); 
	}


	void CheckPhysics()
	{
		Ray ray = new Ray(transform.position, -transform.up); 
		RaycastHit hit; 

		if(Physics.Raycast(ray, out hit, 1.05f*height))
		{
			aerial = false; 
		}
		else
		{
			aerial = true; 
			rb.velocity += Vector3.down*AdditionalGravity; 
		}

	}


	void SkaterMove(Vector2 inputs)
	{
	

		Quaternion PhysicsRotation = aerial ? Quaternion.identity : GetPhysicsRotation(); // Rotation according to ground normal 
		Quaternion VelocityRotation = GetVelocityRot(); 
		Quaternion InputRotation = Quaternion.identity; 
		Quaternion ComputedRotation = Quaternion.identity;


		if(inputs.magnitude > 0.1f)
		{
			Vector3 adapted_direction = CamToPlayer(inputs); 
			Vector3 planar_direction = transform.forward; 
			planar_direction.y = 0; 
			InputRotation = Quaternion.FromToRotation(planar_direction, adapted_direction); 

			if(!aerial)
			{
				Vector3 Direction = InputRotation*transform.forward*Speed; 
				rb.AddForce(Direction); 
			}
		} 

		ComputedRotation = PhysicsRotation*VelocityRotation*transform.rotation; 
		transform.rotation = Quaternion.Lerp(transform.rotation, ComputedRotation, RotatingSpeed*Time.deltaTime); 



	}


	Quaternion GetVelocityRot()
	{
		Vector3 vel = rb.velocity;
		if(vel.magnitude > 0.2f)
		{
			vel.y = 0; 
			Vector3 dir = transform.forward; 
			dir.y = 0; 
			Quaternion vel_rot = Quaternion.FromToRotation(dir.normalized, vel.normalized); 
			return vel_rot; 
		} 
		else
			return Quaternion.identity; 
	}

	Quaternion GetPhysicsRotation()
	{
		Vector3 target_vec = Vector3.up; 
		Ray ray = new Ray(transform.position, Vector3.down); 
		RaycastHit hit; 

		if(Physics.Raycast(ray, out hit, 1.05f*height))
		{
			target_vec = hit.normal; 
		}

		return Quaternion.FromToRotation(transform.up, target_vec); 
	}

	void Rotate(Vector3 d)
	{

	}

	Vector3 CamToPlayer(Vector2 d)
	{
		Vector3 cam_to_player = transform.position - cam.transform.position; 
		cam_to_player.y = 0; 

		Vector3 cam_to_player_right = Quaternion.AngleAxis(90, Vector3.up)*cam_to_player; 

		Vector3 direction = cam_to_player*d.y + cam_to_player_right*d.x; 
		return direction.normalized; 
	}

	void Initialization()
	{
		// cam = Camera.main; 
		// TargetRotation = transform.rotation; 
		// VelocityRotation = Quaternion.identity; 
		rb = GetComponent<Rigidbody>(); 
		inputs = GetComponent<InputProcessing>() ; 

		height = GetComponent<Collider>().bounds.size.y/2f; 
	}
}