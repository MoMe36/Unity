using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSCam : MonoBehaviour
{
	public Transform Target; 
	public float DampSpeed = 2;

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
		LookTarget = Vector3.Lerp(LookTarget, v, 5*Time.deltaTime); 
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
