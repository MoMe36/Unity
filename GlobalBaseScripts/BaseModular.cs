using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseModular : MonoBehaviour {

  BaseFight fight;
  BaseInputs inputs;
  BaseMove move;
  // GoWAxeControl axe_control;

  public bool is_IA = false;
  bool jump_control = false;

  // Use this for initialization
  void Start () {

    if (!is_IA)
      inputs = GetComponent<BaseInputs>();
    fight = GetComponent<BaseFight>();
    move = GetComponent<BaseMove>();
    // axe_control = GetComponent<GoWAxeControl>();

  }

  // Update is called once per frame
  void Update () {


    if(is_IA)
      IAControl();
    else
      PlayerControl();

    ContinuousInformations();


    // AJOUTER AXE AIM: FINIR ANIMATIONS  + LOGIQUE. DANS GOWFIGHT ?

  }

  void IAControl()
  {

  }

  void PlayerControl()
  {

    Vector2 direction = inputs.GetDirection();
    move.PlayerMove(direction, inputs.Jump, inputs.HighSpeed);
    fight.HoldThrow(inputs.HoldThrow); 

    if(inputs.Hit)
      fight.Hit();
    
    // if(inputs.Throw)
    //   axe_control.Throw(); 

    // if(inputs.CallAxe)
    //   axe_control.CallAxe();

    // if(inputs.ChangeWeapon)
    //   axe_control.ChangeWeapon(); 

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
      move.DashState(state); 
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

    // if(info == "GetAxeOver" && !state) // !state to make sure we're exiting the state
    // {
    //   axe_control.GetAxeOver(); // This function sets animation layer 1 weight to 0  
    // }

    // if(info == "ReleaseAxe")
    // {
    //   axe_control.ReleaseAxe(); 
    // }

    // if(info == "ChangeAxeParent")
    // {
    //   axe_control.ChangeAxeParent(); 
    // }

    if(info == "MoveState")
    {
      move.MoveState(state); 
    }
    if(info == "HighSpeedState")
    {
      move.HighSpeedState(state); 
    }

    if(info == "PrepareThrowState")
    {
      move.PrepareThrowState(state); 
    }

    if(info == "UpperCoordination")
    {
      fight.UpperCoordination(state); 
    }

  }

  public void HitInform(BaseHitData hit_data, bool state)
  {
    fight.Switch(hit_data, state);
    if(state)
      move.OtherState(state); 
  }

}
