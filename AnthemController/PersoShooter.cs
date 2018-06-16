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
	bool is_super_flying = false; 
	public bool UseAdditionalGravity = true; 


	bool is_landing = false; 
	Vector3 LandingPosition = Vector3.zero; 
	float LandingSpeed = 5f; 
	float MinSpeedLandingRatio = 1; 


	bool UseStep = false; 
	float AngleBeforeStep =1f; 
	float StepSpeed =1f; 
	float AngleTarget =1f; 
	float MinAngleStep = 1f; 
	bool is_stepping = false; 
	Quaternion StepTarget = Quaternion.identity; 


	bool UseSuperFly = false; 
	float StraightSpeed = 1f; 
	float SuperFlyRotationSpeed = 1f; 
	float VerticalRatio = 0.5f; 

	public bool UseSpecialEffects = false; 
	public List<NamedEffect> FXHolder = new List<NamedEffect>(); 
	int EffectIterator = 0; 
	int MaxEffectIterator; 

	float height; 

	public List <AnimationState> states = new List<AnimationState>(); 
	public List <CharacterStates> c_states = new List<CharacterStates>(); 

	Dictionary<string, AnimationState> dico = new Dictionary<string, AnimationState>(); 
	Dictionary<string, CharacterStates> dico_c_states = new Dictionary<string, CharacterStates>(); 

	public CharacterStates current_c_state; 

	int states_enumerator = 0; 
	int max_enumerator = 0; 


	int debug_counter = 0; 

	public PersoShooter(GameObject o, PersoFillerShooter pfs, Camera c)
	{
		cam = c; 
		G = o; 
		transform = o.transform; 
		rb = o.GetComponent<Rigidbody>(); 
		anim = o.GetComponent<Animator>(); 

		FillFromFiller(pfs); 
		FillDico();
		InitiateImpulsion(); 
		InitiateShooterStates(); 
	}

	void InitiateShooterStates()
	{
		current_c_state = c_states[0]; 
	}

	public void MAJ()
	{
		UpdateStates(); 
		CheckPhysics(); 
		CheckImpuslion(); 
		CheckSpecialEffects();
	}

	void CheckSpecialEffects()
	{
		EffectIterator = (EffectIterator+1)%MaxEffectIterator; 
		FXHolder[EffectIterator].Analyze(current_c_state, rb.velocity.magnitude); 
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
		// EnsurePassive(); //TODO ! FIGURE OUT HOW TO GET OUT OF THIS STATE 

		anim.SetBool(current_state.name,current_state.state);
		// if(current_state.state)
		// 	current_state.state = false; 
		if(current_state.state)
		{
			if(!current_c_state.Passive)
				current_state.state = false; 
		}

		CheckAllCorrespondances();	

	}

	void EnsurePassive()
	{
		if(current_c_state.Passive)
		{
			bool b = false; 
			foreach(string s in current_c_state.Correspondances)
			{
				if(dico.ContainsKey(s))
				{
					dico[s].state = true; 
				}
			}
		}

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

		if(current_c_state.Name == "SuperFly")
		{
			SuperFlyMove(x,y);
		}
		else
		{
			 GroundMove(x,y); 
		}
		
	}


	void SuperFlyMove(float x, float y)
	{


		anim.SetFloat("X", x); 
		anim.SetFloat("Y", y);

		Move(transform.forward*StraightSpeed);
		ApplyAdditionalForce(-y*transform.up*StraightSpeed*0.5f); 
		RotateTowards(transform.forward +  transform.right*x, SuperFlyRotationSpeed); 

	}

	void GroundMove(float x, float y)
	{
		anim.SetFloat("X", x); 
		anim.SetFloat("Y", y);
		if(x*x + y*y > 0.1f)
		{
			// Debug.Log(new Vector3(x,y,0));
			Vector3 direction = transform.forward*y + transform.right*x; 
			// Debug.DrawRay(transform.position, direction*10, Color.red,1f); 
			Move(direction.normalized*Speed); 
			RotateTowards(CamToPlayer(), RotationSpeed); 
			if(is_stepping)
				is_stepping = !is_stepping; 
		}
		else
		{
			if(UseStep)
				StepCamera(); 
		}
	}

	void StepCamera()
	{
		if(!is_stepping)
		{
			Vector3 v = CamToPlayerParallel(); 
			float a = Vector3.Angle(transform.forward, v); 
			// Debug.Log(a); 

			if(a > AngleBeforeStep)
			{
				is_stepping = true; 
				// float d = (Vector3.Dot(transform.right, v) > 0) ? 1f: -1f; 
				// StepTarget = transform.rotation*Quaternion.AngleAxis(d*AngleTarget, transform.up); 

			}	
		}
		else
		{
			Vector3 v = CamToPlayerParallel(); 
			float a = Vector3.Angle(transform.forward, v); 
			// Debug.Log(a); 
			if(a < MinAngleStep)
				is_stepping = false; 

			float d = (Vector3.Dot(transform.right, v) > 0) ? 1f: -1f; 
			StepTarget = transform.rotation*Quaternion.AngleAxis(d*AngleTarget, transform.up); 
			transform.rotation = Quaternion.Lerp(transform.rotation, StepTarget, StepSpeed*Time.deltaTime); 
		}

	}

	public void ToggleSuperFly()
	{
		if(is_flying || current_c_state.Name == "Jump" || is_super_flying)
		{
			Debug.Log(current_c_state.Name); 
			if(current_c_state.Name == "SuperFly")
			{
				Activate("ExitSuperFly"); 
				is_super_flying = false; 
			}
			else
			{
				Activate("EnterSuperFly"); 
				is_super_flying = true; 
				TurnGravityOff(); 
			}
			
		}
		
	}

	Vector3 CamToPlayer()
	{
		Vector3 v = transform.position - cam.transform.position; 
		v.y = 0; 
		return v.normalized; 
	}

	Vector3 CamToPlayerParallel()
	{
		Vector3 v = transform.position - cam.transform.position; 
		v.y = transform.forward.y;  
		return v.normalized; 
	}

	void Move(Vector3 v)
	{
		rb.AddForce(v); 
	}

	void RotateTowards(Vector3 v, float s)
	{
		float angle = Vector3.Angle(transform.forward,v); 
		angle = (Vector3.Dot(transform.right, v) <0) ? -angle: angle; 
		Quaternion target = transform.rotation*Quaternion.AngleAxis(angle, transform.up); 

		transform.rotation = Quaternion.Lerp(transform.rotation, target, s*Time.deltaTime); 
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

		foreach(CharacterStates c in c_states)
		{
			dico_c_states.Add(c.Name, c); 
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
			// c.Reset(); 
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

		UseStep = p.UseStep; 
		AngleBeforeStep = p.AngleBeforeStep;
		StepSpeed = p.StepSpeed;
		AngleTarget = p.AngleTarget;
		MinAngleStep = p.MinAngleStep; 

		AdditionalGravity = p.AdditionalGravity; 
		DragFly = p.DragFly;
		DragGround = p.DragGround;

		LandingSpeed = p.LandingSpeed; 
		MinSpeedLandingRatio = p.MinSpeedLandingRatio; 

		UseSuperFly = p.UseSuperFly; 
		StraightSpeed = p.StraightSpeed; 
		SuperFlyRotationSpeed = p.SuperFlyRotationSpeed; 
		VerticalRatio = p.VerticalRatio; 

		UseSpecialEffects = p.UseSpecialEffects;
		if(UseSpecialEffects)
		{
			FXHolder = p.FXHolder;
			MaxEffectIterator = FXHolder.Count; 
		}


		height = G.GetComponent<Collider>().bounds.size.y/2; 
	}

	public bool GetGrounded()
	{
		return is_grounded; 
	}
}
