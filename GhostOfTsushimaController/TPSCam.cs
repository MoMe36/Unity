using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSCam : MonoBehaviour
{
	[Header("\t Basics")]
	public Transform Target; 
	public float DampSpeed = 2;
	public float LookFocusSpeed = 5; 

	[Space(20)]
	[Header("\t Fighting parameters")]

	public bool FightMode; 
	public Transform EnnemiTarget = null;
	
	[Space(10)]
	public Vector2 DistancesMinMax; 
	public Vector3 CloseOffset;  
	public Vector3 FarOffset;  
	[Range(0f,1f)]
	public float RatioLook; 

	[Space(20)]
	[Header("\t Shooting parameters")]
	public Vector3 ShootOffset; 

	Vector3 Offset; 
	Vector3 velocity= Vector3.zero; 

	float angleY = 0; 
	float angleX = 0; 

	public float Speed = 5; 

	bool is_shooting = false; 
	Vector3 LookTarget; 

	public void Start()
	{
		Offset = transform.position - Target.transform.position; 
		LookTarget = Target.position; 
	}

	public void Update()
	{

		if(FightMode)
		{
			FightBehaviour();
		}
		else
		{
			FreeBehaviour();
		}
		

		
	}
	void FightBehaviour()
	{
		if(EnnemiTarget == null)
		{
			EnnemiTarget = Target.gameObject.GetComponent<SamuraiController>().Ennemi.transform; 
			Debug.Log(EnnemiTarget);
		}

		GetFightingPosition(); 
		

	}

	void LookFight(float r)
	{
		Vector3 v = (Target.position + EnnemiTarget.position)/2f; 
		Vector3 look_target = new Vector3(); 

		LookTarget = v*(1-r) + Target.position*r; 
		// transform.LookAt(look_target);

	}

	void GetFightingPosition()
	{

		Vector3 far_offset = Target.right*FarOffset.x + Target.forward*FarOffset.z - Target.up*FarOffset.y; 
		Vector3 close_offset = Target.right*CloseOffset.x + Target.forward*CloseOffset.z - Target.up*CloseOffset.y; 

		float current_distance = (Target.position - EnnemiTarget.position).magnitude; 
		current_distance = Mathf.Clamp(current_distance, DistancesMinMax.x, DistancesMinMax.y); 
		float ratio = (current_distance - DistancesMinMax.x)/DistancesMinMax.y;

		Vector3 optimal_pos_close = (Target.position + EnnemiTarget.position)/2f + close_offset; 
		Vector3 optimal_pos_far =  Target.position + far_offset; 		 

		// Debug.Log("Distance " + current_distance.ToString() + " Ratio " + ratio.ToString()); 
		// transform.position = optimal_pos_close*(1-ratio) + ratio*optimal_pos_far;
		// RatioLook = ratio; 
		AdaptPosition(optimal_pos_close*(1-ratio) + ratio*optimal_pos_far,DampSpeed);
		LookFight(ratio); 
	}

	void FreeBehaviour()
	{
		float ix = Input.GetAxis("VerCam"); 
		float iy = Input.GetAxis("HorCam"); 


		if (Input.GetAxis("R2") > 0.5f)
		{
			AdaptLookTarget(Target.position + 5*Target.forward);
			AdaptPosition(ShootPos(), DampSpeed/5);

			if(!is_shooting)
			{
				// ResetAngles(); 
				is_shooting = true;
			}
		}

		else
		{
			if(is_shooting)
			{
				is_shooting = false; 
			}

			Vector3 rotated_offset = Rotate(ix*Speed, iy*Speed)*Offset;
			AdaptPosition(Target.position + rotated_offset, DampSpeed);
			AdaptLookTarget(Target.position); 
		}

		RemoveEnnemi(); 
	}

	void RemoveEnnemi()
	{
		if(EnnemiTarget != null)
		{
			EnnemiTarget = null; 
		}
	}

	void LateUpdate()
	{
		transform.LookAt(LookTarget); 		
	}

	void ResetAngles()
	{
		angleY = 0; 
		angleX = 0; 
	}

	void AdaptPosition(Vector3 v, float s)
	{
		transform.position = Vector3.SmoothDamp(transform.position,v, ref velocity, s*Time.deltaTime); 
	}

	void AdaptLookTarget(Vector3 v)
	{
		// LookTarget = Vector3.Lerp(LookTarget, v, LookFocusSpeed*Time.deltaTime); 
		LookTarget = v; 
	}

	Vector3 ShootPos()
	{
		Vector3 pos = Target.position + Target.forward*ShootOffset.z + Target.right*ShootOffset.x + Target.up*ShootOffset.y;
		
		return pos; 
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
