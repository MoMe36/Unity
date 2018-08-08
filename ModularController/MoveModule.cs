using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveModule : MonoBehaviour {


	public float Speed = 1f; 
	public float RotatingSpeed = 1f; 
	public float JumpForce = 1f; 
	public float AdditionalGravity = 3f; 

	[Space(10)]
	[Header("MinStepMax")]
	public float MaxSpeed = 15f; 
	public Vector3 LerpDragSpeed; 

	[Space(10)]
	[Header("Dash parameters")]
	public float DashForce = 1f; 


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

	public void PlayerMove(Vector2 direction)
	{
		CurrentDirection = direction; 
		if(direction.magnitude > 0.1f)
		{
			Move(cam.transform.right*direction.x); 
			Rotate(cam.transform.right*direction.x); 
		}
	}

	public void Dash()
	{
		anim.SetTrigger("Dash"); 
	}

	public void DashMove()
	{
		rb.velocity += transform.forward*DashForce; 
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
		speed_param = Mathf.Clamp01(Mathf.Lerp(speed_param, rb.velocity.magnitude, LerpDragSpeed.y)); 
		anim.SetFloat("Speed", speed_param); 
	}

	void AdjustPhysics()
	{
		Ray ray = new Ray(transform.position, -Vector3.up); 
		RaycastHit hit; 

		if(Physics.Raycast(ray, out hit, height*1.05f))
		{
			if(is_jumping)
			{
				anim.SetTrigger("Land");
			}
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
			rb.velocity += -Vector3.up*AdditionalGravity; 
			LerpDrag(LerpDragSpeed.x);
		}
		else
		{
			if(CurrentDirection.magnitude > 0.2f)
				LerpDrag(LerpDragSpeed.x);
			else
				LerpDrag(LerpDragSpeed.z);	
		}

		// CheckMaxSpeed(); 
	}

	void LerpDrag(float v)
	{
		rb.drag = Mathf.Lerp(rb.drag, v, LerpDragSpeed.z*Time.deltaTime); 
	}

	void CheckMaxSpeed()
	{
		float current_limit = is_jumping ? MaxSpeed*5f : MaxSpeed; 
		// Debug.Log(current_limit); 
		if(rb.velocity.magnitude > MaxSpeed)
		{
			Debug.Log("Speed over " + current_limit.ToString()); 
		}
		Vector3 current_velocity = rb.velocity; 
		current_velocity = (current_velocity.magnitude > current_limit) ? 
						Vector3.Lerp(current_velocity, current_velocity.normalized*current_limit, LerpDragSpeed.y*Time.deltaTime) : current_velocity; 
		rb.velocity = current_velocity; 
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
