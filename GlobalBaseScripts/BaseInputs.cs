using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseInputs : MonoBehaviour {

	public bool UseKeyboard; 
	public float x; 
	public float y; 

	public bool Jump; 
	public bool Hit; 
	public bool CallAxe; 
	public bool Throw; 
	public bool ChangeWeapon; 
	public bool HighSpeed; 
	public bool HoldThrow; 
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if(UseKeyboard)
			ProcessKB(); 
		else
			ProcessJS(); 		
		
	}

	void ProcessKB()
	{
		CallAxe = Input.GetMouseButtonDown(1) ? true : false;  
		Throw = Input.GetMouseButtonDown(0) ? true : false; 
	}

	void ProcessJS()
	{
		x = Input.GetAxis("Horizontal"); 
		y = Input.GetAxis("Vertical"); 


		Jump = Input.GetButtonDown("AButton") ? true : false; 
		Hit = Input.GetButtonDown("R1") ? true : false;
		CallAxe = Input.GetButtonDown("YButton") ? true : false;  
		Throw = Input.GetButtonDown("L1") ? true : false; 
		ChangeWeapon = Input.GetAxis("CroixH") > 0.5f ? true : false; 
		HoldThrow = Input.GetAxis("R2") > 0.5f ? true: false; 
	}


	public Vector2 GetDirection()
	{
		return new Vector2(x,y);
	}
}
