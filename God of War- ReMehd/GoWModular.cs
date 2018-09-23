using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoWModular : MonoBehaviour {

  GoWFight fight;
  GoWInputs inputs;
  GoWMove move;
  GoWAxeControl axe_control;

  public bool is_IA = false;
  bool jump_control = false;

  // Use this for initialization
  void Start () {

    if (!is_IA)
      inputs = GetComponent<GoWInputs>();
    fight = GetComponent<GoWFight>();
    move = GetComponent<GoWMove>();
    axe_control = GetComponent<GoWAxeControl>();

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
    
    if(inputs.Throw)
      axe_control.Throw(); 

    if(inputs.CallAxe)
      axe_control.CallAxe();

    if(inputs.ChangeWeapon)
      axe_control.ChangeWeapon(); 


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
    if(info == "Dash")
    {
      // move.DashMove();
      move.SwitchDash(state);

    }
    // if(info == "Shoot")
    // {
    // 	fight.ShootAction();
    // }
    if(info == "JumpControl")
    {
      jump_control = state;
    }

    if(info == "GetAxeOver" && !state) // !state to make sure we're exiting the state
    {
      axe_control.GetAxeOver(); // This function sets animation layer 1 weight to 0  
    }

    if(info == "ReleaseAxe")
    {
      axe_control.ReleaseAxe(); 
    }

    if(info == "ChangeAxeParent")
    {
      axe_control.ChangeAxeParent(); 
    }
  }

  public void HitInform(HitData hit_data, bool state)
  {
    fight.Switch(hit_data, state);
  }

}
