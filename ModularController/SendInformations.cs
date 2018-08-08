using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendInformations : StateMachineBehaviour {

	public string Information; 
	public bool HasDelay; 
	public float DelayRatio = 0.1f; 

	bool ready = true; 
	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if(!HasDelay)
		{
			Call(animator); 
		}
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if(HasDelay && ready)
		{
			if(stateInfo.normalizedTime >= DelayRatio)
			{
				Call(animator); 
				ready = false; 
			}
		}
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		ready = true; 
	}

	void Call(Animator animator)
	{	
		animator.gameObject.GetComponent<ModularController>().Inform(Information);
	}
	
}
