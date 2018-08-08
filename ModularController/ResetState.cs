using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetState : StateMachineBehaviour {

	public string StateToReset; 
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
		animator.SetBool(StateToReset, false); 
	}
	
}