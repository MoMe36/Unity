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

	DelayedImpulsion impulsion; 

	float RotationTowardsEnnemySpeed = 1f; 

	float DodgeImpulsionStrength = 0.1f; 
	float DodgeImpulsionDuration = 0.1f; 
	float HitImpulsionDuration = 0.1f; 
	float HitImpulsionStrength = 0.1f; 
	float DodgeImpulsionDelay = 0.1f; 
	float HitImpulsionDelay = 0.1f; 

	float HitCounter = 0.1f; 	
	float DodgeCounter = 0.1f; 
	float hit_counter = 0f;
	float dodge_counter = 0f;

	List<ImpulsionHolder> impulsionHolder; 

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
		CheckImpuslion(); 
		if(UseSpecialEffects)
			CheckSpecialEffects();
	}

	void CheckSpecialEffects()
	{
		EffectIterator = (EffectIterator+1)%MaxEffectIterator; 
		FXHolder[EffectIterator].Analyze(current_c_state, rb.velocity.magnitude); 
	}

	void CheckImpuslion()
	{

		impulsion_enumerator = (impulsion_enumerator+1)%max_impulsion_enumerator; 
		bool ac = impulsionHolder[impulsion_enumerator].Analyze(real_current_state); 
		// ac = false; 
		// Debug.Log(ac);
		if(ac)
			impulsion = impulsionHolder[impulsion_enumerator].GetClone(transform); 


		bool b = impulsion.Count(); 
		if (b)
			ApplyImpulsion(); 
	}


	void UpdateStates()
	{
		states_enumerator = (states_enumerator+1)%max_enumerator;
		CheckState();
		CheckCounters(); 

		// Debug.Log(real_current_state);
	}

	void CheckCounters()
	{
		if(dodge_counter >= 0f)
			dodge_counter -= Time.deltaTime; 
		if(hit_counter >= 0f)
			hit_counter -= Time.deltaTime; 
	}

	public void HitActivation()
	{
		// Debug.Log("inse"); 
		if(hit_counter <= 0f)
		{	
			bool b = false; 
			if(current_c_state.FightingState)
				b = Activate("Follow"); 
			else
				b = Activate("Hit");
			
			// if(b)
			// {
			// 	CounterAndImpulsion(transform.forward, HitImpulsionStrength, HitImpulsionDelay, HitImpulsionDuration, hit_counter, HitCounter);
			// }
			
		}
	}

	void CounterAndImpulsion(Vector3 v, float f, float de, float du, float c, float max_c)
	{
		impulsion = new DelayedImpulsion(v,f,de, du); 
		c = max_c; 
	}

	public void Dodge(float x, float y)
	{
		if(dodge_counter <= 0f)
		{
			bool b = Activate("Dodge"); 
			if(b)
			{
				float v = (x > 0f) ? 1f:0f; 
				anim.SetFloat("DodgeDir",v); 
				// if(b)
				// {
				// 	CounterAndImpulsion(transform.right*(v-0.5f)*2, DodgeImpulsionStrength, DodgeImpulsionDelay, DodgeImpulsionDuration, dodge_counter, DodgeCounter);
				// }
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

		DodgeImpulsionStrength = p.DodgeImpulsionStrength;
		DodgeImpulsionDuration = p.DodgeImpulsionDuration;
		DodgeImpulsionDelay = p.DodgeImpulsionDelay;
		HitImpulsionDuration = p.HitImpulsionDuration;
		HitImpulsionStrength = p.HitImpulsionStrength;
		HitImpulsionDelay = p.HitImpulsionDelay;

		HitCounter = p.HitCounter; 
		DodgeCounter = p.DodgeCounter; 

		impulsionHolder = p.impulsionHolder; 

		UseSpecialEffects = p.UseSpecialEffects;
		if(UseSpecialEffects)
		{
			FXHolder = p.FXHolder;
			MaxEffectIterator = FXHolder.Count; 
		}


		height = G.GetComponent<Collider>().bounds.size.y/2; 
	}
}
