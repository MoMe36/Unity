using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inputs2B : MonoBehaviour {

	public bool UseKeyboard; 
	public float x; 
	public float y; 

	public bool Jump; 
	public bool Dash; 
	public bool Hit; 
	public bool Shoot;
	public bool HeavyHit;  


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		ProcessInput(); 
		ProcessButton();
		
	}

	public Vector2 GetDirection()
	{
		return new Vector2(x,y); 
	}

	void ProcessInput()
	{
		if(UseKeyboard)
		{
			x = Input.GetAxis("kHorizontal");
			y = Input.GetAxis("kVertical");

			if(Input.GetKeyDown(KeyCode.K))
			{
				x = 1f; 
			}
			else if (Input.GetKeyDown(KeyCode.H))
			{
				x = -1f; 
			}
			else
			{
				x = 0f; 
			}
		}
		else
		{
			x = Input.GetAxis("Horizontal");
			y = Input.GetAxis("Vertical");
		}
	}

	void ProcessButton()
	{
		if(UseKeyboard)
		{
			if(Input.GetKeyDown(KeyCode.Space))
				Jump = true; 
			else
				Jump =  false; 
		}
		else
		{
			Jump = Check("AButton");
			Dash = Check("BButton");
			Hit = Check("XButton");
			Shoot = Check("R1");
			HeavyHit = Check("YButton"); 
		}
	}

	bool Check(string input_name)
	{
		if(Input.GetButtonDown(input_name))
			return true; 
		else
			return false; 
	}	
}

