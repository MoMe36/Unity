using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasicEnnemiBehaviour
{
	public BasicEnnemiSubBehaviour [] Behaviours;

	public BasicEnnemiBehaviour()
	{

	}


	public string SelectAction(float d)
	{

		// Select subbehaviour based on distance 
		int iterator = 0; 
		int i = 0; 
		foreach(BasicEnnemiSubBehaviour b in Behaviours)
		{
			if(d > b.DistanceMin && d < b.DistanceMax)
			{
				iterator = i; 
			}

			i++; 
		}
		

		// Select action by sampling 
		string s = Behaviours[iterator].Sample(); 

		return s; 
	}





}