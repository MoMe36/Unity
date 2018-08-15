using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamBoxer : MonoBehaviour {

	public Transform Target; 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		transform.LookAt(Target); 
		
	}

	// void FixedUpdate() //Use this one for adapting position 
	// {

	// }
}
