using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NamedEffect
{
	public List<ParticleSystem> Particles = new List<ParticleSystem>(); 
	public List<TrailRenderer> Trails = new List<TrailRenderer>(); 

	[Header("Common Parameters")]
	public Transform DesiredSpot; 
	// public float TimeBeforeDestruction = 0.5f; 
	public string Name = "Effect"; 
	public List<string> Correspondance = new List<string>(); 

	[Header("Particles Parameters")]
	public bool Passive = false; 
	public bool CreateInstance = true;
	public bool OnExit = false; 
	
	[Header("Trail Parameters")]
	public float MinVelocity = 0.5f; 

	[HideInInspector]
	public bool Active = false; 


	bool Loaded = false; 
	bool ready = false; 
	bool trail_active = false; 
	int nb_particles = 0; 
	int nb_trails = 0; 
	float CurrentSpeed = 0f; 
	string current_anim = ""; 


	public NamedEffect()
	{

	}

	public NamedEffect(List<ParticleSystem> p, List<TrailRenderer> t, string n, List<string> c)
	{
		Particles = p; 
		// Trails = t; 
		Name = n; 
		Correspondance = c; 
	}

	void Init()
	{
		ready = true; 
		nb_particles = Particles.Count; 
		nb_trails = Trails.Count; 
	}

	// while it is possible to add a delay to activate, it is also possible to add a delay to the effect ! 

	public void Analyze(string c, float current_speed = 0f)  
	{
		if(!ready)
		{
			Init(); 
		}
		// In case needed by trail 
		CurrentSpeed = current_speed; 

		if(Correspondance.Contains(c))
		{
			if(c != current_anim)
			{
				current_anim = c; 
				Active = false; 
			}
			if(!Active)
			{
				if(OnExit)
					SwitchExit(true); 
				else
					Switch(true); 
			}
		}
		else
		{
			if(OnExit)
			{
				SwitchExit(false); 
			}
			else
			{
				Switch(false); 
			}
		}
	}

	void SwitchExit(bool state)
	{
		if(state)
		{
			Active = true; 
			Loaded = true; 
		}
		else
		{
			if(Loaded)
			{
				Loaded = !Loaded; 
				PlayAllParticles(); 
				Active = false; 
			}
		}
		
	}

	void Switch(bool state)
	{

		Active = state;
		if(Active)
		{
			if(nb_particles > 0)
			{
				PlayAllParticles(); 
			}
			if(nb_trails > 0 && !trail_active && CurrentSpeed > MinVelocity)
			{
				ActivateAllTails(); 
			}
		}
		else
		{
			if(nb_particles > 0)
			{
				StopAllParticles(); 
			}
			if(nb_trails > 0 && trail_active)
			{
				StopAllTrails(); 
			}
		}
	}

	void ActivateAllTails()
	{
		foreach(TrailRenderer t in Trails)
		{
			t.enabled = true; 
		}
		trail_active = true; 
	}

	void StopAllTrails()
	{
		foreach(TrailRenderer t in Trails)
		{
			t.enabled = false; 
		}	
		trail_active = false; 
	}
	void PlayAllParticles()
	{
		if(CreateInstance)
		{
			foreach(ParticleSystem p in Particles)
			{
				ParticleSystem p_clone = Object.Instantiate(p, DesiredSpot.position, DesiredSpot.rotation);
				p_clone.GetComponent<CFX_AutoDestructShuriken>().enabled = true; 
				// Object.Destroy(p_clone, TimeBeforeDestruction); 
			    p_clone.Play(); 

			}
		}
		else
		{
			foreach(ParticleSystem p in Particles)
			{
				p.Play(); 
			}
		}
	}

	void StopAllParticles()
	{
		if(!CreateInstance)
		{
			foreach(ParticleSystem p in Particles)
			{
				p.Stop(); 
			}
		}
		
	}

} 