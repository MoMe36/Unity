using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class BasicEnnemiSubBehaviour
{
	public string Name = "Type"; 
	public float DistanceMin; 
	public float DistanceMax; 
	public StochasticBehaviour [] Actions; 
	

	bool FirstTime= true; 
	float SumProbas = 0f; 
	float [] limits; 

	public BasicEnnemiSubBehaviour()
	{

		// NormalizeAll(); 
	} 

	public void NormalizeAll()
	{
		foreach (StochasticBehaviour sb in Actions)
		{
			SumProbas += sb.Probability; 
		}

		foreach (StochasticBehaviour sb in Actions)
		{
			sb.Probability /= SumProbas; 
		}

		limits = new float [Actions.Length+1]; 

		for(int i = 1; i<limits.Length; i++)
		{
			limits[i] = limits[i-1] + Actions[i-1].Probability; 

		}

		// DEBUG 
		// Debug.Log(Name + " limits: ");
		// foreach(float l in limits)
		// {
		// 	Debug.Log(l); 
		// }

	}


	public string Sample()
	{

		if(FirstTime)
		{
			FirstTime = false; 
			NormalizeAll(); 
		}

		float f = Random.Range(0f, 1f); 
		int selection = 0; 
		for(int i = 1; i < limits.Length; i++)
		{
			if(f > limits[i-1] && f < limits[i])
				selection = i-1; 
		}

		return Actions[selection].Action; 
	}

}
