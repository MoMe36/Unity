using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSkate : MonoBehaviour {

	[Header("\t Basics")]
	public Transform Target; 
	public float DampSpeed = 2;
	public float LookFocusSpeed = 5; 
	public float RotationSpeed = 5; 

	Vector3 Offset; 
	Vector3 velocity= Vector3.zero; 

	float angleY = 0; 
	float angleX = 0; 

	// public float Speed = 5; 

	Vector3 LookTarget; 
	Vector3 TargetPos; 

	// Use this for initialization
	void Start () {
		Offset = transform.position - Target.transform.position; 
		LookTarget = Target.position; 
		TargetPos = transform.position; 
	}
	
	// Update is called once per frame
	void Update () {

		float ix = Input.GetAxis("VerCam"); 
		float iy = Input.GetAxis("HorCam"); 

		Vector3 rotated_offset = Rotate(ix*RotationSpeed, iy*RotationSpeed)*Offset;
		TargetPos = Target.position + rotated_offset; 
		AdaptLookTarget(Target.position); 
		transform.LookAt(LookTarget);
		
	}

	void FixedUpdate()
	{
		AdaptPosition(); 
	}
	
	void ResetAngles()
	{
		angleY = 0; 
		angleX = 0; 
	}

	void AdaptPosition()
	{
		transform.position = Vector3.SmoothDamp(transform.position,TargetPos, ref velocity, DampSpeed*Time.deltaTime); 
	}

	void AdaptLookTarget(Vector3 v)
	{
		LookTarget = v; 
	}

	Quaternion Rotate(float x, float y)
	{
		angleX += x;
		angleY += y; 

		angleX = Mathf.Clamp(angleX,-45, 45);
		Quaternion r = Quaternion.AngleAxis(angleX, transform.right)*Quaternion.AngleAxis(angleY,Vector3.up); 
		return r; 
	}

	
}

