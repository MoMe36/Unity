using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 


public class GoWMove : MonoBehaviour {


	public Text debug_text; 
	string debug_text_data; 
	float session_max_height = 0f; 

	public float max_height; 

	public float Speed = 1f; 
	public float RotatingSpeed = 1f; 
	public float JumpForce = 1f; 
	public float AdditionalGravity = 3f; 
	public float LerpGravitySpeed = 1f; 
	public float LerpAnimSpeed = 1f; 
	float current_additional_gravity = 0f; 

	[Space(10)]
	[Header("MinStepMax")]
	public float MaxSpeed = 15f; 
	public float MaxSpeedJumping = 15f; 
	public float MaxSpeedLerp = 1f; 
	public float LandingAccelerationRatio = 1f; 
	public LerpVector LerpDragSpeed; 


	[Space(10)]
	[Header("Dash parameters")]
	public float DashForce = 1f; 


	[Space(10)]
	[Header("Jump Control data")]
	public float MaxHeight = 2f; 
	public float JumpControlLerpSpeed = 1f; 
	public string JumpControlAnimationName = "Jump"; 
	public float InitialJumpLerpValue = 0.5f; 
	[HideInInspector] public bool is_jumping;
	float jump_lerp_current_value = 0.5f; 


	bool is_moving = false; 
	Vector3 CurrentDirection = Vector3.zero; 

	Quaternion TargetRotation; 

	Animator anim;
	Rigidbody rb; 
	Camera cam; 



	float height; 



	// Use this for initialization
	void Start () {

		Initialization();
		
	}
	
	// Update is called once per frame
	void Update () {

		debug_text_data = ""; 
		session_max_height = transform.position.y > session_max_height ? transform.position.y : session_max_height; 

		AdjustRotation(); 
		AdjustPhysics(); 
		AdjustAnim();

		debug_text_data += "Jumping: "+ is_jumping.ToString(); 
		debug_text_data += "\nCurrent gravity: " + current_additional_gravity.ToString(); 
		debug_text_data += "\nGlobal speed: " + rb.velocity.magnitude.ToString(); 
		debug_text_data += "\nHorizontal speed: " + Vector3.ProjectOnPlane(rb.velocity, Vector3.up).magnitude.ToString(); 
		debug_text_data += "\nMax height so far: " + session_max_height.ToString(); 
		debug_text.text = debug_text_data; 
	}

	void AdjustPhysics()
	{
		AdjustDrag(); 
		CheckGroundContact(); 
		ApplyAdditionalGravity(); 
	}

	void AdjustRotation()
	{
		transform.rotation = Quaternion.Lerp(transform.rotation, TargetRotation, RotatingSpeed*Time.deltaTime); 
	}

	void CheckGroundContact()
	{
		Ray ray = new Ray(transform.position, Vector3.down); 
		RaycastHit hit; 


		if(Physics.Raycast(ray, out hit, 1.1f*height))
		{
			is_jumping = false; 
		}
		else
		{
			is_jumping = true; 
		}
	}

	void ApplyAdditionalGravity()
	{
		if(is_jumping)
		{
			// 1. Lerp current gravity 
			current_additional_gravity = Mathf.Lerp(current_additional_gravity, AdditionalGravity, LerpGravitySpeed*Time.deltaTime); 
			// 2. Apply  
			rb.velocity += Vector3.down*current_additional_gravity;
		}
	}

	void AdjustDrag()
	{
		float target_drag_value =0f; 
		if(is_jumping)
		{
			target_drag_value = LerpDragSpeed.min_value; 
			rb.drag = target_drag_value; 
		}
		else
		{
			target_drag_value = is_moving ? LerpDragSpeed.max_value : LerpDragSpeed.min_value; 
			rb.drag = Mathf.Lerp(rb.drag, target_drag_value, LerpDragSpeed.step_value*Time.deltaTime); 
		}
	}

	void AdjustAnim()
	{
		float current_speed = Mathf.Lerp(anim.GetFloat("Speed"), Vector3.ProjectOnPlane(rb.velocity, Vector3.up).magnitude/3f,LerpAnimSpeed);
		anim.SetFloat("Speed", current_speed); 

	}

	public void JumpControl()
	{
		Ray ray = new Ray(transform.position, - transform.up); 
		RaycastHit hit; 
		
		float current_height = MaxHeight; 
		bool touched = false; 

		if(Physics.Raycast(ray, out hit, MaxHeight))
		{
			current_height = hit.distance; 
			touched = true; 
		}

		float a = 1f/(1.1f*height - MaxHeight); 
		float b = MaxHeight/(MaxHeight - 1.1f*height); 
		float normalized_clip_value = Mathf.Clamp01((current_height*a + b));  

		jump_lerp_current_value = Mathf.Lerp(jump_lerp_current_value, normalized_clip_value, JumpControlLerpSpeed*Time.deltaTime); 

		if(normalized_clip_value > 0.85f && rb.velocity.y <0.1f)
		{
			// anim.SetTrigger("Land"); 
			// SetAdditionalGravity(false); 
			jump_lerp_current_value = InitialJumpLerpValue; 
			current_additional_gravity = 0f; 
			TransferSpeedOnLanding(); 
		}
		else
		{
			anim.Play(JumpControlAnimationName, 0,  jump_lerp_current_value); 
		}

	}

	void TransferSpeedOnLanding()
	{
		Vector3 horizontal_vel = Vector3.ProjectOnPlane(rb.velocity, Vector3.up); 
		if(horizontal_vel.magnitude > 1f)
		{
			rb.velocity += LandingAccelerationRatio*horizontal_vel; 
		}
	}

	public void PlayerMove(Vector2 direction, bool jump)
	{
		CurrentDirection = direction; 
		if(direction.magnitude > 0.1f)
		{
			Move(transform.forward*Speed); 
			Rotate(direction); 

			is_moving = true; 
		}
		else
		{
			is_moving = false; 
		}

		CheckMaxSpeed();
		if(jump)
		{
			Jump(); 
		} 
	}

	void Jump()
	{
		anim.SetTrigger("Jump"); 
		is_jumping = true; 

		rb.velocity += Vector3.up*JumpForce; 
	}

	void Move(Vector3 direction)
	{
		rb.AddForce(direction); 
	}

	void Rotate(Vector2 direction)
	{
		Vector3 look_at_direction = CamToPlayer(direction); 
		TargetRotation = Quaternion.LookRotation(look_at_direction, Vector3.up); 
	}

	// void CheckMaxSpeed() //This function set a max speed but it applies to the whole vector. Which yieds the drawback of blocking the vertical speed
	// {
	// 	float current_limit = is_jumping ? MaxSpeedJumping : MaxSpeed;
	// 	// float current_limit = MaxSpeed;  
	// 	if(rb.velocity.magnitude > MaxSpeed)
	// 	{
	// 		Debug.Log("Speed over " + current_limit.ToString()); 
	// 	}
	// 	Vector3 current_velocity = rb.velocity; 
	// 	current_velocity = (current_velocity.magnitude > MaxSpeed) ? 
	// 					Vector3.Lerp(current_velocity, current_velocity.normalized*current_limit, MaxSpeedLerp*Time.deltaTime) : current_velocity; 
	// 	rb.velocity = current_velocity; 
	// }

	void CheckMaxSpeed()
	{
		Vector3 horizontal_vel = Vector3.ProjectOnPlane(rb.velocity, Vector3.up); 
		if(horizontal_vel.magnitude > MaxSpeed)
			horizontal_vel = Vector3.Lerp(horizontal_vel, horizontal_vel.normalized*MaxSpeed, MaxSpeedLerp*Time.deltaTime);

		horizontal_vel.y = rb.velocity.y; 
		rb.velocity = horizontal_vel; 

	}

	Vector3 CamToPlayer(Vector2 direction)
	{
		Vector3 cam_to_player = Vector3.ProjectOnPlane(transform.position - cam.transform.position ,Vector3.up).normalized; 
		Vector3 rotated_cam_to_player = Quaternion.AngleAxis(90, Vector3.up)*cam_to_player; 

		return cam_to_player*direction.y + rotated_cam_to_player*direction.x; 
	}

	void Initialization()
	{
		cam = Camera.main; 
		rb = GetComponent<Rigidbody>(); 
		anim = GetComponent<Animator>(); 

		TargetRotation = transform.rotation; 

		height = GetComponent<Collider>().bounds.size.y/2f; 
}
}


[System.Serializable]
public struct LerpVector
{
	public float min_value;
	public float max_value;
	public float step_value; 
}