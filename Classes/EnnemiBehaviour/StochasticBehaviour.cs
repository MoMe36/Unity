using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StochasticBehaviour 
{
	public float Probability;
	public string Action; 


	public StochasticBehaviour(float p, string s)
	{
		Probability = p; 
		Action = s; 
	}

}