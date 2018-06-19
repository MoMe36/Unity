using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitBox : MonoBehaviour {

	public Transform TargetPosition; 
	public List<string> ActiveStates = new List<string>(); 
	[HideInInspector]
	public bool Active = false; 

}
