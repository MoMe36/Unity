using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingDodgeInfo : StateMachineBehaviour {

	public string Information = "Dodge"; 
	public string [] HitboxName; 

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		Switch(animator, true); 
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
		animator.SetBool("Left", false); 
		animator.SetBool("Right", false); 
		Switch(animator, false); 
	}

	void Switch(Animator animator, bool state)
	{
		foreach(string s in HitboxName)
		{
			animator.gameObject.GetComponent<BoxerModularController>().SwitchHitbox(s, state, true);
		}
	}
	
}