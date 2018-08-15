using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModularController : MonoBehaviour {

	FightModule fight; 
	Inputs2B inputs; 
	MoveModule move; 

	// Use this for initialization
	void Start () {

		inputs = GetComponent<Inputs2B>(); 
		fight = GetComponent<FightModule>(); 
		move = GetComponent<MoveModule>(); 
	}

	// Update is called once per frame
	void Update () {
		
		bool jump = inputs.Jump; 
		bool dash = inputs.Dash; 
		bool hit = inputs.Hit; 
		bool shoot = inputs.Shoot; 
		bool heavy_hit = inputs.HeavyHit; 

		Vector2 direction = inputs.GetDirection(); 

		move.PlayerMove(direction, jump); 
		fight.Hit(hit, heavy_hit); 
		
		if(dash)
			move.Dash(); 

		if(shoot)
			fight.Shoot(); 
	}

	public void Inform(string info)
	{
		if(info == "Dash")
		{
			move.DashMove(); 
		}
		if(info == "Shoot")
		{
			fight.ShootAction(); 
		}
		if(info == "JumpControl")
		{
			move.JumpControl(); 
		}
	}

	public void Switch(string info, bool state)
	{
		if(info == "LF" || 
		   info == "RH" ||
		   info == "RF" ||
		   info == "LH" )
		{
			fight.Switch(info, state);
		}
	}

}
