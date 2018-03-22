using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Detector 
{

	[HideInInspector]
	public GameObject Holder; 
	[HideInInspector]
	public GameObject Ennemi; 
	public float Distance = 10; 
	public float MaxAngle = 180; 
	public float Angle = 0;

	[HideInInspector]
	public LayerMask Mask; 
	[HideInInspector]
	public float Increment = 11;  
	[HideInInspector]	
	public bool HasEnnemi = false; 

	public Detector(GameObject G, LayerMask l)
	{
		Holder = G; 
		Mask = l ; 
	}

	public bool Observe()
	{
		Vector3 dir = Quaternion.AngleAxis(Angle, Vector3.up)*Holder.transform.forward;
		Ray ray = new Ray(Holder.transform.position,dir);

		// Debug.DrawRay(ray.origin, ray.direction*Distance, Color.red, 1.0f); 
		RaycastHit hit; 

		bool b = false; 

		if(Physics.Raycast(ray, out hit, Distance, Mask))
		{
			Ennemi = hit.transform.gameObject;
			b = true; 
			Debug.Log("Found one"); 
		}

		Angle = (Angle + Increment > MaxAngle) ? -MaxAngle + (Angle+Increment)%MaxAngle : Angle + Increment; 
		return b; 
	}

	public GameObject GetEnnemi()
	{
		return Ennemi; 
	}






}