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
		Quaternion r = Quaternion.AngleAxis(angleX, Target.right)*Quaternion.AngleAxis(angleY,Target.up); 
		return r; 
	}

	
}


// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class TPSCam : MonoBehaviour
// {
	


// 	public void Update()
// 	{

// 		if(FightMode)
// 		{
// 			FightBehaviour();
// 		}
// 		else
// 		{
// 			FreeBehaviour();
// 		}
		

		
// 	}
// 	void FightBehaviour()
// 	{
// 		if(EnnemiTarget == null)
// 		{
// 			EnnemiTarget = Target.gameObject.GetComponent<SamuraiController>().Ennemi.transform; 
// 			Debug.Log(EnnemiTarget);
// 		}

// 		GetFightingPosition(); 
		

// 	}

// 	void LookFight(float r)
// 	{
// 		Vector3 v = (Target.position + EnnemiTarget.position)/2f; 
// 		Vector3 look_target = new Vector3(); 

// 		LookTarget = v*(1-r) + Target.position*r; 
// 		// transform.LookAt(look_target);

// 	}

// 	void GetFightingPosition()
// 	{

// 		Vector3 far_offset = Target.right*FarOffset.x + Target.forward*FarOffset.z - Target.up*FarOffset.y; 
// 		Vector3 close_offset = Target.right*CloseOffset.x + Target.forward*CloseOffset.z - Target.up*CloseOffset.y; 

// 		float current_distance = (Target.position - EnnemiTarget.position).magnitude; 
// 		current_distance = Mathf.Clamp(current_distance, DistancesMinMax.x, DistancesMinMax.y); 
// 		float ratio = (current_distance - DistancesMinMax.x)/DistancesMinMax.y;

// 		Vector3 optimal_pos_close = (Target.position + EnnemiTarget.position)/2f + close_offset; 
// 		Vector3 optimal_pos_far =  Target.position + far_offset; 		 

// 		// Debug.Log("Distance " + current_distance.ToString() + " Ratio " + ratio.ToString()); 
// 		// transform.position = optimal_pos_close*(1-ratio) + ratio*optimal_pos_far;
// 		// RatioLook = ratio; 
// 		AdaptPosition(optimal_pos_close*(1-ratio) + ratio*optimal_pos_far,DampSpeed);
// 		LookFight(ratio); 
// 	}

	

// 	void RemoveEnnemi()
// 	{
// 		if(EnnemiTarget != null)
// 		{
// 			EnnemiTarget = null; 
// 		}
// 	}

	

// 	Vector3 ShootPos()
// 	{
// 		Vector3 pos = Target.position + Target.forward*ShootOffset.z + Target.right*ShootOffset.x + Target.up*ShootOffset.y;
		
// 		return pos; 
// 	}

	


// }