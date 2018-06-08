using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PersoFiller 
{
	[Header("Attributes")]
	public float Speed = 10; 
	public float RotationSpeed = 2; 
	public float JumpForce = 350; 
	public float MaxVelocity = 1000;
	public float DashForce = 3;
	public float DashTime = 0.5f; 
	public float AdditionalGravity = 5f; 
	public float DragFly = 1f; 
	public float DragGround = 2f; 

	[Header("Attributes/Animation")]
	public Transform Armature; 
	public bool UsePivot = true; 
	public float PivotDelay = 0.2f; 
	public bool UseLean = true; 
	public float MaxLeaningAngle = 15; 
	public float LeanSpeed = 1f; 
	public bool UseJumpOver = false; 
	public Vector3 ImpulsionComponents = new Vector3(0, 1f, 0.1f);
	public float ImpulsionDuration, ImpulsionStrength, ImpulsionDelay, DetectionDistance; 


	[Header("Character States")]
	public List <CharacterStates> c_states = new List<CharacterStates>();

	[Header("Animations States")]
	public List <AnimationState> states = new List<AnimationState>();


}
