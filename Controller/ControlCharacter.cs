using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCharacter : MonoBehaviour {

	public PersoFiller Filler; 
	Perso me; 

	public bool UseDebug = false; 

	// Use this for initialization
	void Start () {

		me = new Perso(gameObject, Filler, Camera.main); 
		
	}
	
	// Update is called once per frame
	void Update () {

		me.MAJ();

		float x = Input.GetAxis("Horizontal"); 
		float y = Input.GetAxis("Vertical"); 

		Vector3 RunningDirection = new Vector3(x,0,y); 
		if(RunningDirection.magnitude > 0.2f)
		{
			me.Activate("Run"); 
			me.PlayerMove(new Vector3(x,0,y)); 
		}

		if(Input.GetButtonDown("XButton"))
		{
			me.Activate("Jump");
			me.Jump(); 
		}


		DebugFunctions(); 
		
	}

	void DebugFunctions()
	{
		if(UseDebug)
		{
			UseDebug = false; 
			me = new Perso(gameObject, Filler, Camera.main);
		}
	}
}
