using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDecisionTree : MonoBehaviour {

	public DecisionTree DecisionModule; 
	public GSR game_state_representation; 
	public float customvalue = 0.5f;

	float timer = 0f; 



	// Use this for initialization
	void Start () {
		

		// Debug.Log(DecisionModule.ToString()); 
	}
	
	// Update is called once per frame
	void Update () {
		
		timer -= Time.deltaTime;
		if (timer <=0f)
		{
			game_state_representation.state[0] = customvalue;
			
			string result = DecisionModule.Decide(game_state_representation);
			Debug.Log(result);
			timer = 1;
		}
	}
}
