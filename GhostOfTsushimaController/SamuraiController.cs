using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamuraiController : MonoBehaviour {

	public bool DEBUG = false; 

	[Header("\t DashTesting")]
	public bool is_dashing = false;
	public float dashspeed = 5f; 
	public float dashdistance = 3; 
	public float dashduration = 1f; 

	public float dashcounter = 0f; 
	Vector3 DashTarget; 

	public GameObject Ennemi; 
	public SamuraiFiller Filler; 
	PersoSamurai me; 



	// Use this for initialization
	void Start () {
		me = new PersoSamurai(gameObject, Filler, Camera.main); 
		me.SetEnnemi(Ennemi); 
	}
	
	// Update is called once per frame
	void Update () {
		
		me.MAJ(); 
		float x = Input.GetAxis("Horizontal"); 
		float y = Input.GetAxis("Vertical"); 

		me.PlayerMove(x,y); 
		
		
		// if(Input.GetKeyDown(KeyCode.Space))
		if(Input.GetButtonDown("XButton"))
			me.HitActivation();
		if(Input.GetButtonDown("BButton"))
			me.Dodge(x,y); 

		if(Input.GetButtonDown("YButton"))
			me.ActivateDash() ;

		// DashEffect(); 
		DebugFunc(); 

	}

	void LaunchDash()
	{
		Debug.Log("Entering dash"); 
		if(!is_dashing)
		{
			dashcounter = dashduration; 
			is_dashing = true; 
			GetDashPos();
			me.Activate("Dash"); 
		}
	}
	void DashEffect()
	{
		if(is_dashing)
		{
			dashcounter -= Time.deltaTime; 
			if(dashcounter <= 0f)
			{
				is_dashing = false; 
			}
			// transform.position += transform.forward*dashspeed*Time.deltaTime;
			transform.position = Vector3.Lerp(transform.position, DashTarget, Time.deltaTime*dashspeed);
		}	
	}

	void GetDashPos()
	{
		Ray ray = new Ray(transform.position, transform.forward); 
		RaycastHit hit; 

		if(Physics.Raycast(ray, out hit, dashdistance))
		{
			DashTarget = transform.position + transform.forward*0.5f*hit.distance; 
		}
		else
		{
			DashTarget = transform.position + transform.forward*dashdistance;
		}
	}

	void DebugFunc()
	{
		if(DEBUG)
		{	
			DEBUG = !DEBUG; 
		}
	}
}
