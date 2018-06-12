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
	public float AngleBeforeStep = 100f; 


	[Header("Fly Attributes")]
	public bool UseFlight = false; 
	public float LandingSpeed = 5f; 
	public float MinSpeedLandingRatio = 3; 



	public List <AnimationState> AnimationStates = new List<AnimationState>(); 
	public List <CharacterStates> CharacterState = new List<CharacterStates>(); 


}