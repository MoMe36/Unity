using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetStates : StateMachineBehaviour {

	public string [] StatesToReset; 
	public bool OnEnter; 

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if(OnEnter)
		{
			SwitchOff(animator);
		}
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if(!OnEnter)
		{
			SwitchOff(animator);
		} 
	}

	void SwitchOff(Animator animator)
	{
		foreach(string s in StatesToReset)
		{
			animator.SetBool(s, false); 
		}
	}
	
}