using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour {

	public string TargetAnimation = "Impact"; 
	public bool Active; 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerStay(Collider other) // if problems: switch to OnTriggerStay 
	{
		// Add conditions 
		if(!(other.transform.root == transform.root))
		{
			if(other.GetComponent<AttackHitBox>())
			{
				if(other.GetComponent<AttackHitBox>().Active)
				{
					Active = true; 
				}
			}
		}
		else
		{
			Debug.Log("Mine"); 
		}
	}
}
