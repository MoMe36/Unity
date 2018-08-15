using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingHitInfo : StateMachineBehaviour {

	public string Information = "HitEnd"; 
	public float ExitTime = 0.9f; 
	public string HitboxName; 
	public bool AddForceOnEnter = false; 

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		Switch(animator, true); 
		if(AddForceOnEnter)
		{
			CallImpulsion(animator); 
		}
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if(stateInfo.normalizedTime > ExitTime)
		{
			Call(animator); 
		}
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.SetBool("Left", false); 
		animator.SetBool("Right", false); 
		Switch(animator, false); 
	}

	void Call(Animator animator)
	{	
		animator.gameObject.GetComponent<BoxerModularController>().Inform(Information);
	}

	void CallImpulsion(Animator animator)
	{
		animator.gameObject.GetComponent<BoxerModularController>().Inform("Impulsion");
	}

	void Switch(Animator animator, bool state)
	{
		animator.gameObject.GetComponent<BoxerModularController>().SwitchHitbox(HitboxName, state);
	}
	
}