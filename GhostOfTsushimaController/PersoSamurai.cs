using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersoSamurai 
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

	bool UseAdditionalGravity; 
	bool is_grounded; 
	bool is_flying;

	DelayedImpulsion impulsion; 

	float RotationTowardsEnnemySpeed = 1f; 

	float DodgeImpulsionStrength = 0.1f; 
	float DodgeImpulsionDuration = 0.1f; 
	float HitImpulsionDuration = 0.1f; 
	float HitImpulsionStrength = 0.1f; 
	float DodgeImpulsionDelay = 0.1f; 
	float HitImpulsionDelay = 0.1f; 

	float TimeBetweenHits; 
	float hit_counter = 0f; 
	bool hit_ready = true; 

	public bool UseDash;
	public float DashSpeed;
	public float DashDistance;
	public float DashDuration;

	bool is_dashing = false; 
	Vector3 DashTarget; 
	float DashCounter;

	bool UseImpulsionAction; 
	bool UseHitboxes;
	List<ImpulsionHolder> impulsionHolder; 
	List <HitBox> Hitboxes; 
	List <AttackHitBox> AttackHitboxes; 
	string LastAttackType = ""; 

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
	public string real_current_state; 

	int states_enumerator = 0; 
	int max_enumerator = 0; 

	int impulsion_enumerator = 0; 
	int max_impulsion_enumerator = 0; 

	int hitbox_enumerator = 0; 
	int max_hitbox_enumerator = 0; 

	int attack_hitbox_enumerator = 0; 
	int max_attack_hitbox_enumerator = 0; 

	public GameObject Ennemy = null; 

	public PersoSamurai(GameObject o, SamuraiFiller sf, Camera c)
	{
		cam = c; 
		G = o; 
		transform = o.transform; 
		rb = o.GetComponent<Rigidbody>(); 
		anim = o.GetComponent<Animator>(); 

		FillFromFiller(sf); 
		FillDico();
		InitiateImpulsion(); 
		InitiateStates(); 
	}

	void InitiateStates()
	{
		current_c_state = c_states[0]; 
		real_current_state = states[0].name; 
	}

	public void MAJ()
	{
		UpdateStates(); 
		CheckImpulsion(); 
		CheckPhysics(); 
		CheckDash(); 
		if(UseSpecialEffects)
			CheckSpecialEffects();
		CheckCounters();
	}

	void CheckCounters()
	{
		if(!hit_ready)
		{
			hit_counter -= Time.deltaTime; 
			if(hit_counter <= 0)
				hit_ready = true; 
		}
	}

	void CheckSpecialEffects()
	{
		EffectIterator = (EffectIterator+1)%MaxEffectIterator; 
		FXHolder[EffectIterator].Analyze(real_current_state, rb.velocity.magnitude); 
	}

	void CheckDash()
	{
		if(UseDash)
		{
			if(is_dashing)
			{
				DashCounter -= Time.deltaTime; 
				if(DashCounter <= 0f)
				{
					is_dashing = false; 
				}
				// transform.position += transform.forward*dashspeed*Time.deltaTime;
				transform.position = Vector3.Lerp(transform.position, DashTarget, Time.deltaTime*DashSpeed);
			}
		}
	}

	public void ActivateDash()
	{
		if(!is_dashing)
		{
			is_dashing = true; 
			DashCounter = DashDuration; 
			Activate("Dash"); 
			GetDashTarget(); 
		}
	}

	void GetDashTarget()
	{
		Ray ray = new Ray(transform.position, transform.forward); 
		RaycastHit hit; 

		if(Physics.Raycast(ray, out hit, DashDistance))
		{
			DashTarget = transform.position + transform.forward*0.5f*hit.distance; 
		}
		else
		{
			DashTarget = transform.position + transform.forward*DashDistance;
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

	void CheckImpulsion()
	{
		if(UseImpulsionAction)
		{
			impulsion_enumerator = (impulsion_enumerator+1)%max_impulsion_enumerator; 
			bool ac = impulsionHolder[impulsion_enumerator].Analyze(real_current_state); 
			if(ac)
				impulsion = impulsionHolder[impulsion_enumerator].GetClone(transform); 
		}


		bool b = impulsion.Count(); 
		if (b)
			ApplyImpulsion(); 
	}


	void UpdateStates()
	{
		states_enumerator = (states_enumerator+1)%max_enumerator;
		CheckState();

		if(UseHitboxes)
			CheckHitboxes(); 
	}

	void CheckHitboxes()
	{
		hitbox_enumerator = (hitbox_enumerator+1)%max_hitbox_enumerator; 
		if(Hitboxes[hitbox_enumerator].Active)
		{
			Activate(Hitboxes[hitbox_enumerator].TargetAnimation); 
			Hitboxes[hitbox_enumerator].Active = false; 
		}

		attack_hitbox_enumerator = (attack_hitbox_enumerator+1)%max_attack_hitbox_enumerator; 
		if(AttackHitboxes[attack_hitbox_enumerator].ActiveStates.Contains(real_current_state))
		{
			AttackHitboxes[attack_hitbox_enumerator].Active = true; 
		}
		else
		{
			AttackHitboxes[attack_hitbox_enumerator].Active = false; 
		}
	}

	public void HitActivation(string anim_name)
	{
		bool b = false; 
		if(hit_ready)
		{
			hit_ready = false; 
			hit_counter = TimeBetweenHits; 
			anim.SetBool("ExitBool", false);
			
			if(anim_name == LastAttackType)
			{
				if(current_c_state.FightingState)
				{
					b = Activate("Follow"); 
				}
				else
				{
					b = Activate(anim_name);
				}
			}
			else
			{
				b = Activate(anim_name);
				LastAttackType = anim_name;
			}

		}
	}

	void CounterAndImpulsion(Vector3 v, float f, float de, float du, float c, float max_c)
	{
		impulsion = new DelayedImpulsion(v,f,de, du); 
		c = max_c; 
	}

	public void Dodge(float x, float y)
	{
			bool b = Activate("Dodge"); 
			if(b)
			{
				float v = (x > 0f) ? 1f:0f; 
				anim.SetFloat("DodgeDir",v); 
			}
	}


	void CheckState()
	{
		AnimationState current_state = states[states_enumerator];

		anim.SetBool(current_state.name,current_state.state);
		if(current_state.state)
		{
			if(!current_c_state.Passive)
				current_state.state = false; 
		}

		CheckAllCorrespondances();	
	}


	public void SetEnnemi(GameObject e)
	{
		Ennemy = e; 
	}

	void ApplyAdditionalForce(Vector3 v)
	{
		rb.AddForce(v); 
	}

	public void PlayerMove(float x, float y)
	{
		GroundMove(x,y); 
		if(Ennemy != null)
		{
			RotationTowardEnnemi();
		}
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
		}
	}

	void RotationTowardEnnemi()
	{
		Vector3 v = Ennemy.transform.position - transform.position; 
		v.y = 0; 
		float angle = Vector3.SignedAngle(transform.forward, v, transform.up); 

		Quaternion target = transform.rotation*Quaternion.AngleAxis(angle, transform.up); 
		transform.rotation = Quaternion.Lerp(transform.rotation, target, RotationTowardsEnnemySpeed*Time.deltaTime); 
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

	void RotateTowards(Vector3 v, float s)
	{
		float angle = Vector3.Angle(transform.forward,v); 
		angle = (Vector3.Dot(transform.right, v) <0) ? -angle: angle; 
		Quaternion target = transform.rotation*Quaternion.AngleAxis(angle, transform.up); 

		transform.rotation = Quaternion.Lerp(transform.rotation, target, s*Time.deltaTime); 
	}

	

	void ApplyImpulsion()
	{
		rb.velocity += impulsion.Impulse(); 
	}


	void InitiateImpulsion()
	{
		impulsion = new DelayedImpulsion(Vector3.zero, 0f,0f,0f); 
		max_impulsion_enumerator = impulsionHolder.Count; 
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

	public bool Activate(string s)
	{
		if(current_c_state.Passive)
		{
			dico[s].state = true;
			return true; 
		}
		else if(current_c_state.Name != s)
		{	
			dico[s].state = true; 
			return true; 
		}
		else
			return false; 
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
					real_current_state = s;  
				}
			}
			c.Current = b; 
			// c.Reset(); 
		}	
	}

	public void FillFromFiller(SamuraiFiller p)
	{
		states = p.AnimationStates; 
		c_states = p.CharacterState;

		Speed = p.Speed; 
		RotationSpeed = p.RotationSpeed; 
		JumpForce = p.JumpForce; 
		MaxVelocity = p.MaxVelocity; 
		DashForce = p.DashForce; 
		DashTime = p.DashTime; 
		RotationTowardsEnnemySpeed = p.RotationTowardsEnnemySpeed; 
		UseAdditionalGravity = p.UseAdditionalGravity; 

		DodgeImpulsionStrength = p.DodgeImpulsionStrength;
		DodgeImpulsionDuration = p.DodgeImpulsionDuration;
		DodgeImpulsionDelay = p.DodgeImpulsionDelay;
		HitImpulsionDuration = p.HitImpulsionDuration;
		HitImpulsionStrength = p.HitImpulsionStrength;
		HitImpulsionDelay = p.HitImpulsionDelay;

		UseDash = p.UseDash; 
		DashSpeed = p.DashSpeed; 
		DashDistance = p.DashDistance; 
		DashDuration = p.DashDuration; 

		TimeBetweenHits = p.TimeBetweenHits; 

		UseImpulsionAction = p.UseImpulsionAction; 
		UseHitboxes = p.UseHitboxes; 

		impulsionHolder = p.impulsionHolder; 

		Hitboxes = p.Hitboxes; 
		AttackHitboxes = p.AttackHitboxes; 

		max_attack_hitbox_enumerator = AttackHitboxes.Count; 
		max_hitbox_enumerator = Hitboxes.Count; 

		UseSpecialEffects = p.UseSpecialEffects;
		if(UseSpecialEffects)
		{
			FXHolder = p.FXHolder;
			MaxEffectIterator = FXHolder.Count; 
		}


		height = G.GetComponent<Collider>().bounds.size.y/2; 
	}
}
