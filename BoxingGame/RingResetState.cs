using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingResetState : StateMachineBehaviour {

	public string Information = "HitEnd"; 
	public bool OnEnter; 

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if(OnEnter)
			Call(animator); 
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	// override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	// 	if(stateInfo.normalizedTime > ExitTime)
	// 	{
	// 		Call(animator); 
	// 	}
	// }

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if(!OnEnter)
			Call(animator); 
	}

	void Call(Animator animator)
	{	
		animator.gameObject.GetComponent<BoxerModularController>().Inform(Information);
	}
	
}