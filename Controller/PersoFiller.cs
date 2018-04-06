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


	[Header("Character States")]
	public List <CharacterStates> c_states = new List<CharacterStates>();

	[Header("Animations States")]
	public List <AnimationState> states = new List<AnimationState>();


}
