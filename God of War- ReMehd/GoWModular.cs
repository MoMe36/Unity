using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoWModular : MonoBehaviour {

	GoWFight fight;
	GoWInputs inputs;
	GoWMove move;

	public bool is_IA = false;
	bool jump_control = false;

	// Use this for initialization
	void Start () {

		if (!is_IA)
			inputs = GetComponent<GoWInputs>();
		fight = GetComponent<GoWFight>();
		move = GetComponent<GoWMove>();

	}

	// Update is called once per frame
	void Update () {


		if(is_IA)
			IAControl();
		else
			PlayerControl();

		ContinuousInformations();

	}

	void IAControl()
	{

	}

	void PlayerControl()
	{
		Vector2 direction = inputs.GetDirection();
		move.PlayerMove(direction, inputs.Jump);

		if(inputs.Hit)
			fight.Hit();
	}

	void ContinuousInformations()
	{
		if(jump_control)
		{
			move.JumpControl();
		}
	}

	public void Inform(string info, bool state)
	{
		// if(info == "Dash")
		// {
		// 	move.DashMove();
		// }
		// if(info == "Shoot")
		// {
		// 	fight.ShootAction();
		// }
		if(info == "JumpControl")
		{
			jump_control = state;
		}
	}

	public void HitInform(HitData hit_data, bool state)
	{
		fight.Switch(hit_data, state);
	}
}
