using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxerModularController : MonoBehaviour {

	InputsBoxer inputs; 
	BoxerMoveController move; 
	BoxerFightController fight; 

	// Use this for initialization
	void Start () {

		inputs = GetComponent<InputsBoxer>(); 
		move = GetComponent<BoxerMoveController>();
		fight = GetComponent<BoxerFightController>();
		
	}
	
	// Update is called once per frame
	void Update () {

		ParsePlayerInputs(); 
		
	}

	void ParsePlayerInputs()
	{
		if(inputs.enabled)
		{
			bool dash = inputs.Dash; 
			Vector2 direction = inputs.GetDirection(); 

			move.PlayerMove(direction, dash); 
			fight.Act(inputs.Left, inputs.Right, inputs.Direct, inputs.Hook, inputs.Upper, inputs.Dodge); 
		}
	}

	public void Inform(string info)
	{
		if(info == "Dash")
		{
			move.ApplyDashForce(); 
		}
		else if(info == "HitEnd")
		{
			fight.BeReady(); 
		}
		else if(info == "Impulsion")
		{
			fight.Impulse(); 
		}
	}

	public void SwitchHitbox(string hitbox_name, bool state, bool dodge = false)
	{
		fight.Switch(hitbox_name, state, dodge);
	}
}
