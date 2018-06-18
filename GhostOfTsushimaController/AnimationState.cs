using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimationState {

	public string name; 
	public bool state; 

	public AnimationState (string s)
	{
		name = s; 
		state = false; 
	}


}
