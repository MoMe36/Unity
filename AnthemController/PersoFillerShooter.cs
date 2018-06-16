using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]	
public class PersoFillerShooter
{
	[Header("Physics Attributes")]
	public float Speed = 200; 
	public float RotationSpeed = 10; 
	public float JumpForce = 5; 
	public float MaxVelocity = 1000; 
	public float DashForce = 3;
	public float DashTime = 0.5f; 
	public float AdditionalGravity = 1f; 
	public float DragFly = 1f;
	public float DragGround = 1f; 

	[Header("Speficic Attributes")]
	public bool UseStep = false; 
	public float AngleBeforeStep = 30; 
	public float StepSpeed = 5; 
	public float AngleTarget = 45; 
	public float MinAngleStep = 5f; 


	[Header("Fly Attributes")]
	public bool UseFlight = false; 
	public float LandingSpeed = 5f; 
	public float MinSpeedLandingRatio = 3; 

	[Header("SuperFly Attributes")]
	public bool UseSuperFly = false; 
	public float StraightSpeed = 5f; 
	public float SuperFlyRotationSpeed = 3f;
	public float VerticalRatio = 0.5f; 


	[Header("Special Effects")]
	public bool UseSpecialEffects = true; 
	public List<NamedEffect> FXHolder = new List<NamedEffect>(); 


	public List <AnimationState> AnimationStates = new List<AnimationState>(); 
	public List <CharacterStates> CharacterState = new List<CharacterStates>(); 


}