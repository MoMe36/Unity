using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersoShooter 
{

	Camera cam; 
	public GameObject G; 
	public GameObject A; 
	public Transform transform; 
	public Rigidbody rb;
	public Animator anim; 

	public float Speed = 200; 
	public float RotationSpeed = 10; 
	public float JumpForce = 5; 
	public float MaxVelocity = 1000; 
	public float DashForce = 3;
	public float DashTime = 0.5f; 
	public float AdditionalGravity = 1f; 
	public float DragFly = 1f;
	public float DragGround = 1f; 

	DelayedImpulsion impulsion; 

	bool is_grounded = true; 
	bool is_falling = false; 
	bool is_flying = false; 
	public bool UseAdditionalGravity = true; 


	bool is_landing = false; 
	Vector3 LandingPosition = Vector3.zero; 
	float LandingSpeed = 5f; 
	float MinSpeedLandingRatio = 1; 

	float height; 

	public List <AnimationState> states = new List<AnimationState>(); 
	public List <CharacterStates> c_states = new List<CharacterStates>(); 

	Dictionary<string, AnimationState> dico = new Dictionary<string, AnimationState>(); 

	public CharacterStates current_c_state; 

	int states_enumerator = 0; 
	int max_enumerator = 0; 



	public PersoShooter(GameObject o, PersoFillerShooter pfs, Camera c)
	{
		cam = c; 
		G = o; 
		transform = o.transform; 
		rb = o.GetComponent<Rigidbody>(); 
		anim = o.GetComponent<Animator>(); 

		Debug.Log("I am a PersoShooter"); 

		FillFromFiller(pfs); 
		FillDico();
		InitiateImpulsion(); 
	}


	public void MAJ()
	{
		UpdateStates(); 
		CheckPhysics(); 
		CheckImpuslion(); 
	}

	void UpdateStates()
	{

		states_enumerator = (states_enumerator+1)%max_enumerator;
		CheckState(); 
		CheckOtherStates(); 

	}

	void CheckOtherStates()
	{
		if(is_landing)
		{
			transform.position = Vector3.Lerp(transform.position, LandingPosition, LandingSpeed*Time.deltaTime); 
			if((transform.position - LandingPosition).magnitude < height*1.2f)
			{
				is_landing = false; 
			}
		}
	}

	void CheckState()
	{
		AnimationState current_state = states[states_enumerator];
		anim.SetBool(current_state.name,current_state.state);
		if(current_state.state)
			current_state.state = false; 

		CheckAllCorrespondances();
	}

	void CheckPhysics()
	{
		bool result = false; 

		Ray ray = new Ray(transform.position, -transform.up); 
		RaycastHit hit; 

		// Debug.DrawRay(ray.origin,ray.direction*height,Color.red,1.0f); 

		if(Physics.Raycast(ray, out hit, height*1.05f))
		{
			result = true; 
		}

		if(result)
		{
			if(!is_grounded)
			{
				is_grounded = true; 
				anim.SetTrigger("TouchedGround"); 
				// Debug.Log("Landing");
				rb.drag = DragGround; 
			}
		}

		else
		{
			if(is_grounded)
			{
				anim.SetTrigger("Fall");
				anim.ResetTrigger("TouchedGround"); 
				is_grounded = false;
				// Debug.Log("Falling");
				rb.drag = DragFly; 
			}
		}

		// Now speed

		rb.velocity = Vector3.ClampMagnitude(rb.velocity, MaxVelocity);
		// Debug.Break();

		if(!is_grounded && UseAdditionalGravity && !is_flying)
			ApplyAdditionalForce(-transform.up*AdditionalGravity);
	}

	void ApplyAdditionalForce(Vector3 v)
	{
		rb.AddForce(v); 
	}

	public void PlayerMove(float x, float y)
	{
		anim.SetFloat("X", x); 
		anim.SetFloat("Y", y);

		if(x*x + y*y > 0.1f)
		{
			// Debug.Log(new Vector3(x,y,0));
			Vector3 direction = transform.forward*y + transform.right*x; 
			// Debug.DrawRay(transform.position, direction*10, Color.red,1f); 
			Move(direction.normalized*Speed); 
			RotateTowards(CamToPlayer()); 
			
		}
	}

	Vector3 CamToPlayer()
	{
		Vector3 v = transform.position - cam.transform.position; 
		v.y = 0; 
		return v.normalized; 
	}

	void Move(Vector3 v)
	{
		rb.AddForce(v); 
	}

	void RotateTowards(Vector3 v)
	{
		float angle = Vector3.Angle(transform.forward,v); 
		angle = (Vector3.Dot(transform.right, v) <0) ? -angle: angle; 
		Quaternion target = transform.rotation*Quaternion.AngleAxis(angle, transform.up); 

		transform.rotation = Quaternion.Lerp(transform.rotation, target, RotationSpeed*Time.deltaTime); 
	}

	public void Jump(float delay = 0.1f)
	{
		if(is_grounded)
			impulsion = new DelayedImpulsion(Vector3.up, JumpForce, 0.3f, delay); 
		// rb.velocity += Vector3.up*JumpForce;
		// rb.AddForce(transform.up*JumpForce);
	}

	public void Fly()
	{
		if(!is_flying && !is_grounded)
		{
			TurnGravityOff(); 
			Activate("Fly"); 
			is_flying = true; 
		}
		else
		{
			is_flying = false; 
			bool [] param = LaunchLandingProcedure(); 
			if(param[0])
			{
				TurnGravityOn(); 
				if(param[1])
				{
					anim.SetTrigger("FlyToLand"); 	
					is_landing = true; 	
				}
				else
				{
					anim.SetTrigger("Fall"); 
				}
			}
		}
	}

	bool [] LaunchLandingProcedure()
	{
		bool granted = false; 
		Ray ray = new Ray(transform.position, Vector3.down); 
		RaycastHit hit; 

		if(Physics.Raycast(ray, out hit))
		{
			LandingPosition = hit.point;
			granted = true; 
		}

		bool fast = ((transform.position - LandingPosition).magnitude > MinSpeedLandingRatio*height)? true:false; 
		bool [] result = new bool [] {granted, fast}; 
		return result; 
	}

	public void ActivateJump()
	{
		if(is_grounded)
		{
			Activate("Jump"); 
			Jump(); 
		}
	}


	public void CheckImpuslion()
	{
		bool b = impulsion.Count(); 
		if (b)
			ApplyImpulsion(); 
	}

	void ApplyImpulsion()
	{
		rb.velocity += impulsion.Impulse(); 
	}


	void InitiateImpulsion()
	{
		impulsion = new DelayedImpulsion(Vector3.zero, 0f,0f,0f); 
	}

	public void FillDico()
	{
		foreach(AnimationState s in states)
		{
			dico.Add(s.name, s); 
		}

		max_enumerator = states.Count; 
	}

	public void Activate(string s)
	{
		if(current_c_state.Passive)
			dico[s].state = true;
		else if(current_c_state.Name != s)
			dico[s].state = true; 
	}

	public void CheckAllCorrespondances()
	{
		foreach(CharacterStates c in c_states)
		{
			bool b = false; 
			foreach(string s in c.Correspondances)
			{
				if(anim.GetCurrentAnimatorStateInfo(0).IsName(s))
				{
					b = true; 
					current_c_state = c; 
				}
			}
			c.Current = b; 
		}	
	}

	public void TurnGravityOff()
	{
		rb.useGravity = false; 
		UseAdditionalGravity = false; 
	}

	public void TurnGravityOn()
	{
		rb.useGravity = true; 
		UseAdditionalGravity = true; 
	}

	public void FillFromFiller(PersoFillerShooter p)
	{
		states = p.AnimationStates; 
		c_states = p.CharacterState;

		Speed = p.Speed; 
		RotationSpeed = p.RotationSpeed; 
		JumpForce = p.JumpForce; 
		MaxVelocity = p.MaxVelocity; 
		DashForce = p.DashForce; 
		DashTime = p.DashTime; 

		AdditionalGravity = p.AdditionalGravity; 
		DragFly = p.DragFly;
		DragGround = p.DragGround;

		LandingSpeed = p.LandingSpeed; 
		MinSpeedLandingRatio = p.MinSpeedLandingRatio; 

		height = G.GetComponent<Collider>().bounds.size.y/2; 
	}

	public bool GetGrounded()
	{
		return is_grounded; 
	}
}
