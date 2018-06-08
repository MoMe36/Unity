using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCharacter : MonoBehaviour {

	public PersoFiller Filler; 
	Perso me; 

	public float JumpDelay = 0.1f; 
	public bool UseDebug = false; 

	// public GameObject Armature; 

	// Quaternion InitalRotation; 
	// float ArmatureAngle; 

	// public Vector3 Obj; 
	// public Vector3 Obj2; 
	// public float LerpSpeed; 
	// Use this for initialization
	public float hitDistance = 1f; 
	void Start () {

		me = new Perso(gameObject, Filler, Camera.main); 
		// InitalRotation = Armature.transform.localRotation; 
		
	}
	
	// Update is called once per frame
	void Update () {

		me.MAJ();

		float x = Input.GetAxis("Horizontal"); 
		float y = Input.GetAxis("Vertical"); 

		// float x = Input.GetAxis("HorKey"); 
		// float y = Input.GetAxis("VerKey"); 

		Vector3 RunningDirection = new Vector3(x,0,y); 
		if(RunningDirection.magnitude > 0.2f)
		{
			me.Activate("Run"); 
			me.PlayerMove(new Vector3(x,0,y)); 
			// CheckPivot(RunningDirection);
			// RotateAroundY(RunningDirection); 
		}

		if(Input.GetButtonDown("AButton"))
		{
			me.Activate("Jump");
			me.Jump(JumpDelay); 
		}

		if(Input.GetButtonDown("XButton"))
		{
			me.Activate("FootShoot");
		}

		// if(Input.GetButtonDown("YButton"))
		// {
		// 	me.Activate("JumpOver"); 
		// }


		// DebugFunctions(); 
		// LerpRotation(); 

		// CheckJumpOver(); 
	}

	void CheckJumpOver()
	{
		Ray ray = new Ray(transform.position, transform.forward); 
		RaycastHit hit; 

		Debug.DrawRay(ray.origin, ray.direction*hitDistance, Color.red ,1); 

		if(Physics.Raycast(ray, out hit, hitDistance))
		{
			if(!me.GetGrounded())
			{
				me.Activate("JumpOver"); 
				me.CustomImpulsion(0.1f*transform.forward+transform.up, 10.5f, 0.0f,0.1f); 
			}
		}
	}

	// void LerpRotation()
	// {
	// 	if(Input.GetButton("YButton"))
	// 	{
	// 		Armature.transform.localRotation = Quaternion.RotateTowards(Armature.transform.localRotation, Quaternion.Euler(Obj), 5);
	// 	}
	// 	else if(Input.GetButton("BButton"))
	// 	{
	// 		Armature.transform.localRotation = Quaternion.RotateTowards(Armature.transform.localRotation, Quaternion.Euler(Obj2), 5);
	// 	}
	// 	else
	// 	{
	// 		Armature.transform.localRotation = Quaternion.RotateTowards(Armature.transform.localRotation, InitalRotation, LerpSpeed);
	// 	}
	// }

	// void RotateAroundY(Vector3 v)
	// {
	// 	v = me.CamToPlayer(v); 
	// 	float angle = Vector3.Angle(transform.forward, v); 
	// 	if(angle > 15)
	// 	{
	// 		angle = (Vector3.Dot(transform.right, v) > 0) ? 1 : -1; 
	// 		ArmatureAngle += angle*Time.deltaTime*TiltAngle; 
			

	// 		Quaternion target = Quaternion.AngleAxis(25*angle, transform.forward);

	// 		Armature.transform.localRotation = Quaternion.RotateTowards(Armature.transform.rotation, target, 5); 
	// 	}
	// 	// transform.rotation = Quaternion.RotateTowards(transform.rotation, target.rotation, step); !!!!!!!
	// 	Armature.transform.localRotation = Quaternion.Lerp(Armature.transform.localRotation, InitalRotation, Time.deltaTime); 
	// 	// J'ESSAIE DE RAJOUTER LEANING SUIVANT LA COURSE ! 	A FINIR ! 
	// }

	void DebugFunctions()
	{
		if(UseDebug)
		{
			UseDebug = false; 
			me = new Perso(gameObject, Filler, Camera.main);
		}
	}

}
