using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DelayedImpulsion 
{

	public Vector3 Direction; 
	public float Strengh; 
	public float Duration; 
	public bool Active = false; 
	public float Delay =0f; 
	float counter = 0.0f;

	public DelayedImpulsion(Vector3 v, float f, float de, float du)
	{
		Direction = v; 
		Strengh = f; 
		Delay = de; 
		Duration = du; 

		if(Delay > 0f)
		{
			Active = false; 
			counter = 0.0f; 
		}
		else
		{
			Active = true; 
			counter = Duration; 
		}
	}

	public bool Count()
	{
		if(Active)
		{
			counter -= Time.deltaTime; 
			if(counter < 0f)
			{
				Active = false;
			}
		}
		else
		{
			if(Delay >0f)
			{
				Delay -= Time.deltaTime; 
				if(Delay <= 0f)
				{
					Active = true; 
					counter = Duration;
				}
			}
		}
		return Active; 
	}

	public Vector3 Impulse()
	{
		return Direction*Strengh; 
	}



}
