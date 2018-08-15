using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputsBoxer : MonoBehaviour {

	public float x, y; 
	public bool Dash; 
	public bool Left; 
	public bool Right; 
	public bool Upper; 
	public bool Direct; 
	public bool Hook; 
	public bool Dodge; 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		x = Input.GetAxis("Horizontal");
		y = Input.GetAxis("Vertical"); 

		Dash = CheckButton("AButton"); 
		Left = CheckAxis("L2"); 
		Right = CheckAxis("R2"); 

		Upper = CheckButton("YButton"); 
		Direct = CheckButton("XButton"); 
		Hook = CheckButton("BButton"); 
		Dodge = CheckButton("R1"); 

	}

	bool CheckAxis(string axis)
	{
		return Input.GetAxis(axis) > 0.5f ? true : false; 
	}

	bool CheckButton(string button)
	{
		if(Input.GetButtonDown(button))
			return true; 
		else
			return false; 
	}

	public Vector2 GetDirection()
	{
		return new Vector2(x,y);
	}
}
