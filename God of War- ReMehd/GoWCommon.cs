using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoWCommon : MonoBehaviour {

	static float TimeFactor = 0.01f; 
	static float StopLength = 0.2f; 
	static float stop_length_counter = 0f; 
	static float inc_value = 0.1f; 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if(stop_length_counter >= 0f)
		{
			stop_length_counter -= inc_value; 
			if(stop_length_counter <= 0f)
			{
				ResetTimeScale(); 
			}
		}
		
	}

	void ResetTimeScale()
	{
		Time.timeScale = 1f; 
		Time.fixedDeltaTime = 0.02f; 
	}

	public static void StopTime()
	{
		Time.timeScale *= TimeFactor; 
		Time.fixedDeltaTime = 0.02f*TimeFactor; 

		stop_length_counter = StopLength; 
	}
}
