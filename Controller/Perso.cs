using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Perso 
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

	Impulsion impulsion; 

	bool is_grounded = true; 
	bool is_falling = false; 

	float height; 

	public List <AnimationState> states = new List<AnimationState>(); 
	public List <CharacterStates> c_states = new List<CharacterStates>(); 

	Dictionary<string, AnimationState> dico = new Dictionary<string, AnimationState>(); 

	public CharacterStates current_c_state; 

	int states_enumerator = 0; 
	int max_enumerator = 0; 


	public Perso(GameObject Holder, GameObject Avatar, PersoFiller p)
	{
		G = Holder; 
		A = Avatar; 
		transform = G.GetComponent<Transform>(); 
		rb = G.GetComponent<Rigidbody>(); 
		anim = Avatar.GetComponent<Animator>(); 


		FillFromFiller(p); 
		FillDico();
		InitiateImpulsion(); 
	}

	public Perso(GameObject Holder, GameObject Avatar, PersoFiller p, Camera c)
	{
		G = Holder; 
		A = Avatar; 
		transform = G.GetComponent<Transform>(); 
		rb = G.GetComponent<Rigidbody>(); 
		anim = Avatar.GetComponent<Animator>(); 
		cam = c; 

		FillFromFiller(p); 
		FillDico();
		InitiateImpulsion(); 
	}


	public Perso(GameObject g, PersoFiller p)
	{
		G = g; 
		transform = g.GetComponent<Transform>(); 
		rb = g.GetComponent<Rigidbody>(); 
		anim = g.GetComponent<Animator>(); 


		FillFromFiller(p);
		FillDico(); 
		InitiateImpulsion();
	}

	public Perso(GameObject g, PersoFiller p, Camera c)
	{
		G = g; 
		transform = g.GetComponent<Transform>(); 
		rb = g.GetComponent<Rigidbody>(); 
		anim = g.GetComponent<Animator>(); 
		cam = c;

		FillFromFiller(p);
		FillDico(); 
		InitiateImpulsion();
	}


	public void MAJ()
	{	
		UpdateStates(); 
		CheckPhysics(); 
		CheckImpulsion();

		if(current_c_state != null)
			Debug.Log(current_c_state.Name);
	}

	void UpdateStates()
	{
		states_enumerator = (states_enumerator+1)%max_enumerator; 
		CheckState(); 
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
		// First Check ground collision 

		bool result = false; 

		Ray ray = new Ray(transform.position, -transform.up); 
		RaycastHit hit; 

		Debug.DrawRay(ray.origin,ray.direction*height,Color.red,1.0f); 

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
				is_grounded = false;
				// Debug.Log("Falling");
				rb.drag = DragFly; 
			}
		}

		// Now speed

		rb.velocity = Vector3.ClampMagnitude(rb.velocity, MaxVelocity);
		// Debug.Break();

		if(!is_grounded)
			ApplyAdditionalForce(-transform.up*AdditionalGravity);
	}

	void ApplyAdditionalForce(Vector3 v)
	{
		rb.AddForce(v);
	}

	public void CheckImpulsion()
	{
		bool b = impulsion.Count(); 
		if (b)
			ApplyImpulsion(); 
	}

	public void ApplyImpulsion()
	{
		// rb.AddForce(impulsion.Impulse()); 
		rb.velocity += impulsion.Impulse();
	}

	public void HitImpulsion()
	{
		rb.velocity += transform.forward;
	}

	public void Dash(Vector3 v)
	{
		v = CamToPlayer(v); 
		impulsion = new Impulsion(v, DashForce, DashTime); 
	}

	public void Jump()
	{
		impulsion = new Impulsion(Vector3.up, JumpForce, 0.3f); 
		// rb.velocity += Vector3.up*JumpForce;
		// rb.AddForce(transform.up*JumpForce);
	}

	public bool IsJumping()
	{
		return !is_grounded; 
	}

	public void PlayerShootMove(Vector3 v, Quaternion r)
	{
		ShootMove(v,r); 
		// Add camera movement ? 
	}

	public void PlayerMove(Vector3 v)
	{
		if(current_c_state.Name == "Run" || current_c_state.Name == "Jump")
		{
			Vector3 transformed_v = CamToPlayer(v); 
			Move(transformed_v); 
		}
		
	}

	public void ShootMove(Vector3 v, Quaternion r)
	{
		if(v.magnitude > 0.3f)
		{
			v = CamToPlayer(v);
			rb.AddForce(v*Speed); 
		}
		transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation*r, RotationSpeed*Time.deltaTime);
	}

	public void Move(Vector3 v)
	{
		rb.AddForce(transform.forward*Speed);
		Rotate(v); 
	}

	void Rotate(Vector3 v)
	{
		float angle = Vector3.Angle(transform.forward, v); 
		angle = (Vector3.Dot(transform.right, v) < 0) ? -angle:angle; 

		Quaternion rot = transform.rotation*Quaternion.AngleAxis(angle,transform.up); 
		transform.rotation = Quaternion.Lerp(transform.rotation, rot, RotationSpeed*Time.deltaTime); 
	}

	public void Activate(string s)
	{
		if(current_c_state.Passive)
			dico[s].state = true;
		else if(current_c_state.Name != s)
			dico[s].state = true; 
	}

	public Vector3 CamToPlayer(Vector3 v)
	{
		Vector3 d = transform.position - cam.transform.position; 
		d.y = 0; 
		d = d.normalized; 

		Vector3 dd = Quaternion.AngleAxis(90, Vector3.up)*d; 
		Vector3 direction = v.x*dd + v.z*d;
		return direction.normalized;
	}

	public void FillDico()
	{
		foreach(AnimationState s in states)
		{
			dico.Add(s.name, s); 
		}

		max_enumerator = states.Count; 
	}

	public void FillFromFiller(PersoFiller p)
	{
		states = p.states; 
		c_states = p.c_states;

		Speed = p.Speed; 
		RotationSpeed = p.RotationSpeed; 
		JumpForce = p.JumpForce; 
		MaxVelocity = p.MaxVelocity; 
		DashForce = p.DashForce; 
		DashTime = p.DashTime; 

		AdditionalGravity = p.AdditionalGravity; 
		DragFly = p.DragFly;
		DragGround = p.DragGround;

		height = G.GetComponent<Collider>().bounds.size.y/2; 
	}

	public void InitiateImpulsion()
	{
		impulsion = new Impulsion(Vector3.zero, 0,0); 
	}

	public void CheckAllCorrespondances()
	{
		foreach( CharacterStates c in c_states)
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

	public List<CharacterStates> GetAllCharacterStates()
	{
		return c_states; 
	}


}