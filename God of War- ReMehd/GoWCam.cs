using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoWCam : MonoBehaviour {

	public Transform Target; 
	public float DampSpeed = 2;
	public float LookFocusSpeed = 5; 
	public float RotationSpeed = 5; 
	public Vector2 LimX; 

	Vector3 Offset; 
	Vector3 velocity= Vector3.zero; 

	float angleY = 0; 
	float angleX = 0; 
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

		transform.LookAt(Target); 
		float ix = Input.GetAxis("VerCam"); 
		float iy = Input.GetAxis("HorCam"); 
		

		Vector3 rotated_offset = Rotate(ix*RotationSpeed, iy*RotationSpeed)*Offset;
		TargetPos = Target.position + rotated_offset; 
		transform.LookAt(Target.position);
		
	}

	void FixedUpdate()
	{
		AdaptPosition(); 
	}
	void AdaptPosition()
	{
		transform.position = Vector3.SmoothDamp(transform.position,TargetPos, ref velocity, DampSpeed*Time.deltaTime); 
	}

	Quaternion Rotate(float x, float y)
	{
		angleX += x;
		angleY += y; 

		angleX = Mathf.Clamp(angleX,LimX.x, LimX.y);
		Quaternion r = Quaternion.AngleAxis(angleX, transform.right)*Quaternion.AngleAxis(angleY,Vector3.up); 
		return r; 
	}
}