using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShooterController : MonoBehaviour {

	public PersoFillerShooter Filler; 
	PersoShooter me; 


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
	}
}
