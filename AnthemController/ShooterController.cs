using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShooterController : MonoBehaviour {

	public bool DEBUG = false; 
	public PersoFillerShooter Filler; 
	PersoShooter me; 


	bool is_rotating = false; 
	// Use this for initialization
	void Start () {

		me = new PersoShooter(gameObject, Filler, Camera.main); 
		
	}
	
	// Update is called once per frame
	void Update () {

		me.MAJ(); 

		float x = Input.GetAxis("Horizontal"); 
		float y = Input.GetAxis("Vertical"); 

		
		me.PlayerMove(x,y); 

		if(Input.GetButtonDown("XButton"))
		{
			me.ActivateJump(); 
			me.Fly(); 
		}

		if(Input.GetButtonDown("YButton"))
		{
			me.ToggleSuperFly(); 
		}

		// if(!is_rotating)
		// {
		// 	TestStepToRotate(); 
		// }
		// else
		// {
		// 	Rotate(); 
		// }

		UseDebug(); 
	}

	void UseDebug()
	{
		if(DEBUG)
		{
			me = new PersoShooter(gameObject, Filler, Camera.main); 
			DEBUG = false; 
		}
	}


	void TestStepToRotate()
	{
		Vector3 v = transform.position - Camera.main.transform.position; 
		v.y = transform.forward.y; 
		float angle = Vector3.Angle(transform.forward, v);
		if (angle > 30)
		{
			is_rotating = true; 
		}
	}

	void Rotate()
	{
		Vector3 v = transform.position - Camera.main.transform.position; 
		v.y = transform.forward.y; 
		float angle = Vector3.Angle(transform.forward, v);
		if (angle < 2)
		{
			is_rotating = false; 
		}
		else
		{
			angle = (Vector3.Dot(transform.right, v) > 0) ? 45:-45; 
			Quaternion target = transform.rotation*Quaternion.AngleAxis(angle, transform.up); 
			transform.rotation = Quaternion.Lerp(transform.rotation, target, 5*Time.deltaTime); 
		}
	}
}
