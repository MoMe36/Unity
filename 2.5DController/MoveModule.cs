using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 


public class MoveModule : MonoBehaviour {

	public Text debug_text; 
	// public bool DebugBreak = false; 
	public float max_height; 

	public float Speed = 1f; 
	public float RotatingSpeed = 1f; 
	public float JumpForce = 1f; 
	public float AdditionalGravity = 3f; 
	public float LerpGravitySpeed = 1f; 
	float current_additional_gravity = 0f; 

	[Space(10)]
	[Header("MinStepMax")]
	public float MaxSpeed = 15f; 
	public float MaxSpeedJumping = 15f; 
	public float LandingAccelerationRatio = 1f; 
	public Vector3 LerpDragSpeed; 


	[Space(10)]
	[Header("Dash parameters")]
	public float DashForce = 1f; 


	[Space(10)]
	[Header("Jump Control data")]
	public float MaxHeight = 2f; 
	public float JumpControlLerpSpeed = 1f; 
	public string JumpControlAnimationName = "Jump"; 
	public float InitialJumpLerpValue = 0.5f; 
	float jump_lerp_current_value = 0.5f; 


	Vector3 CurrentDirection = Vector3.zero; 

	Animator anim;
	Rigidbody rb; 
	Camera cam; 

	Quaternion initial_rotation; 
	Quaternion other_rotation; 
	Quaternion target; 

	[HideInInspector] public bool is_jumping;

	float height; 

	// Use this for initialization
	void Start () {

		Initialization();
		
	}
	
	// Update is called once per frame
	void Update () {

		AdjustPhysics(); 
		AdjustRotation(); 
		AdjustAnim(); 


		
	}

	public void PlayerMove(Vector2 direction, bool jump)
	{
		CurrentDirection = direction; 
		if(direction.magnitude > 0.1f)
		{
			Move(cam.transform.right*direction.x); 
			Rotate(cam.transform.right*direction.x); 
		}

		CheckMaxSpeed();
		if(jump)
		{
			Jump(); 
		} 
	}

	void CheckMaxSpeed()
	{
		float current_limit = is_jumping ?MaxSpeedJumping : MaxSpeed;
		// float current_limit = MaxSpeed;  
		if(rb.velocity.magnitude > MaxSpeed)
		{
			Debug.Log("Speed over " + current_limit.ToString()); 
		}
		Vector3 current_velocity = rb.velocity; 
		current_velocity = (current_velocity.magnitude > MaxSpeed) ? 
						Vector3.Lerp(current_velocity, current_velocity.normalized*current_limit, LerpDragSpeed.y*Time.deltaTime) : current_velocity; 
		rb.velocity = current_velocity; 
	}

	public void Dash()
	{
		anim.SetTrigger("Dash"); 
	}

	public void DashMove()
	{
		rb.velocity += transform.forward*DashForce; 
	}

	public void JumpControl()
	{
		Ray ray = new Ray(transform.position, - transform.up); 
		RaycastHit hit; 
		// Debug.DrawRay(transform.position, - transform.up*MaxHeight, Color.red, Time.deltaTime); 
		// Debug.DrawRay(transform.position - transform.forward*0.5f, - transform.up*height, Color.green, Time.deltaTime); 

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

		string s = "Height: " + height.ToString(); 
		s += "\nHit distance: " + current_height.ToString() + "  Touched: " + touched.ToString(); 
		s += "\nNormalized value: " + normalized_clip_value.ToString(); 
		s += "\nJump value: " + jump_lerp_current_value.ToString(); 

		debug_text.text = s; 

		if(normalized_clip_value > 0.95f && rb.velocity.y <0.1f)
		{
			anim.SetTrigger("Land"); 
			SetAdditionalGravity(false); 
			jump_lerp_current_value = InitialJumpLerpValue; 
			TransferSpeedOnLanding(); 
		}
		else
		{
			if(!anim.GetBool("DJump"))
				anim.Play(JumpControlAnimationName, 0,  jump_lerp_current_value); 
		}

		// THEN Add wall jump 
		// THEN More work on the ball 
		
	}

	public void TransferSpeedOnLanding()
	{
		float magn_vel = rb.velocity.magnitude;
		Vector3 new_vel = Vector3.ProjectOnPlane(rb.velocity, Vector3.up); 
		new_vel = new_vel.normalized*magn_vel;

		rb.velocity +=  LandingAccelerationRatio*new_vel; 
	}

	void Jump()
	{
		if(is_jumping)
		{
			anim.SetBool("DJump", true); 
			Debug.Log("Calling djump"); 
		}
		else
		{
			anim.SetTrigger("Jump"); 
		}
		is_jumping = true;
		rb.velocity += transform.up*JumpForce; 
	}

	void Move(Vector3 direction)
	{
		rb.AddForce(direction*Speed);		
	}

	void Rotate(Vector3 direction)
	{
		target = (Vector3.Dot(direction, cam.transform.right) > 0) ? initial_rotation : other_rotation; 
	}

	void AdjustRotation()
	{
		transform.rotation = Quaternion.Lerp(transform.rotation, target, RotatingSpeed*Time.deltaTime); 
	}

	void AdjustAnim()
	{
		float speed_param = anim.GetFloat("Speed"); 
		Vector3 horizontal_vel = Vector3.ProjectOnPlane(rb.velocity, Vector3.up); 
		speed_param = Mathf.Clamp01(Mathf.Lerp(speed_param, horizontal_vel.magnitude, LerpDragSpeed.y)); 
		anim.SetFloat("Speed", speed_param); 
	}

	void AdjustPhysics()
	{
		Ray ray = new Ray(transform.position, -Vector3.up); 
		RaycastHit hit; 

		if(Physics.Raycast(ray, out hit, height*1.1f))
		{
			is_jumping = false; 
		}
		else
		{
			if(!is_jumping)
			{
				anim.SetTrigger("Fall"); 
			}
			is_jumping = true; 
		}


		if(is_jumping)
		{
			SetAdditionalGravity(true); 
			rb.velocity += -Vector3.up*current_additional_gravity; 
			LerpDrag(LerpDragSpeed.x);
		}
		else
		{
			if(CurrentDirection.magnitude > 0.2f)
				LerpDrag(LerpDragSpeed.x);
			else
				LerpDrag(LerpDragSpeed.z);	
		}

	}

	void LerpDrag(float v)
	{
		rb.drag = Mathf.Lerp(rb.drag, v, LerpDragSpeed.z*Time.deltaTime); 
	}

	void SetAdditionalGravity(bool state)
	{
		if(state)
		{
			current_additional_gravity = Mathf.Lerp(current_additional_gravity, AdditionalGravity, LerpGravitySpeed*Time.deltaTime); 
		}
		else
		{
			current_additional_gravity = 0f; 
		}
	}

	void Initialization()
	{
		cam = Camera.main; 
		rb = GetComponent<Rigidbody>(); 
		anim = GetComponent<Animator>(); 

		initial_rotation = transform.rotation; 
		other_rotation = Quaternion.AngleAxis(180, Vector3.up); 
		target = initial_rotation; 

		height = GetComponent<Collider>().bounds.size.y/2f; 
	}
}
