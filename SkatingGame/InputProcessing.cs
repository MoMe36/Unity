using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class InputProcessing : MonoBehaviour {

	public float X;
	public float Y; 


	// Use this for initialization
	void Start () {

		

	}
	
	// Update is called once per frame
	void Update () {
		
		X = Input.GetAxis("Horizontal");  
		Y = Input.GetAxis("Vertical"); 

	}


	public Vector2 GetDirection()
	{
		return new Vector2(X,Y); 
	}

}