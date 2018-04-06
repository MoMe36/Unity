using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impulsion 
{
	public Vector3 Direction; 
	public float Strengh; 
	public float Duration; 
	public bool Active = false; 

	float counter = 0.0f; 

	public Impulsion(Vector3 v, float f, float d)
	{
		Direction = v; 
		Strengh = f; 
		Duration = d; 

		counter = Duration; 
		Active = true; 
	}

	public bool Count()
	{
		if(Active)
		{
			if(counter > 0)
			{
				counter -= Time.deltaTime; 
			}	
			if (counter < 0)
				Active = false; 
		}
			
		return Active; 
	}

	public Vector3 Impulse()
	{
		return Direction*Strengh; 
	}
}
