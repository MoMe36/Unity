using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestExploDT : MonoBehaviour {

	public DecisionTree DecisionModule; 

	// Use this for initialization
	void Start () {

		// Debug.Log(DecisionModule); 
		// Debug.Log(DecisionModule.nodes); 
		// Debug.Log(DecisionModule.nodes.Count); 
		// Debug.Log(DecisionModule.nodes[0]); 
		// Debug.Log(DecisionModule.nodes[1]); 
		// Debug.Log(DecisionModule.nodes[2]);

		Debug.Log(DecisionModule.nodes[0]);  
		Debug.Log(DecisionModule.nodes[0].true_node);  
		Debug.Log(DecisionModule.nodes[0].false_node);  

		// Debug.Log(DecisionModule.genealogy.Keys); 


		GSR s = new GSR(); 
		s.state = new float []{0.1f}; 
		string action = DecisionModule.Decide(s); 

		Debug.Log(action); 
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
