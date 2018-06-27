using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SamuraiFiller
{
	[Header("\t\tPhysics Attributes")]
	public float Speed = 200; 
	public float RotationSpeed = 10; 
	public float JumpForce = 5; 
	public float MaxVelocity = 1000; 
	public float DashForce = 3;
	public float DashTime = 0.5f; 
	public float AdditionalGravity = 1f; 
	public float DragFly = 1f;
	public float DragGround = 1f; 
	public bool UseAdditionalGravity; 

	[Space(50)]

	[Header("\t\tSpecific Attributes")]
	public float RotationTowardsEnnemySpeed = 1f;
	public float DodgeImpulsionStrength = 0.5f; 
	public float DodgeImpulsionDuration = 0.1f; 
	public float DodgeImpulsionDelay = 0.1f; 
	public float HitImpulsionStrength = 0.1f; 
	public float HitImpulsionDuration = 0.1f;
	public float HitImpulsionDelay = 0.1f; 

	[Space(20)]
	[Header("\t\tDash Attributes")]
	public bool UseDash = true; 
	public float DashSpeed = 5f; 
	public float DashDistance = 5;
	public float DashDuration = 0.4f; 


	[Space(50)]

	[Header("\t\tImpulsions during actions")]
	public bool UseImpulsionAction = false; 
	public List<ImpulsionHolder> impulsionHolder = new List<ImpulsionHolder>(); 

	[Space(50)]

	[Header("\t\tHitboxes")]
	public bool UseHitboxes = false; 
	public List<HitBox> Hitboxes = new List<HitBox>(); 
	public List <AttackHitBox> AttackHitboxes = new List <AttackHitBox> (); 

	[Space(50)]

	[Header("\t\tSpecial Effects")]
	public bool UseSpecialEffects = true; 
	public List<NamedEffect> FXHolder = new List<NamedEffect>(); 

	[Space(50)]

	[Header("\t\tAnimation Parameters")]
	public List <AnimationState> AnimationStates = new List<AnimationState>(); 

	[Space(30)]
	public List <CharacterStates> CharacterState = new List<CharacterStates>(); 

}
