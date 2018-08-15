using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitImpact : MonoBehaviour {

	[Header("True = Impact | False = Hit")]
	public bool TypeImpact = false; 
	public bool Active = false; 
	public GameObject ImpactEffect; 

	TrailRenderer trail;
	FightModule holder;

	[HideInInspector] public float HeavyForce; 
	[HideInInspector] public float LightForce; 


	// Use this for initialization
	void Start () {

		Initialization(); 
			
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other)
	{
		if(TypeImpact)
		{
			HitImpact other_hit = other.gameObject.GetComponent<HitImpact>(); 
			if(other_hit != null)
			{
				if(other_hit.Active)
				{
					Impacted(other.transform.position, other_hit.HeavyForce); 
				}
			}
		}

	}

	void Impacted(Vector3 pos, float force)
	{
		Debug.Log(force); 
		GameObject p = Instantiate(ImpactEffect, pos, ImpactEffect.transform.rotation) as GameObject; 
		holder.Impacted(force); 
	}

	public void ParticleImpact()
	{
		Impacted(transform.position, 150); 
	}

	public void Switch(bool state)
	{
		Active = state; 
		if(Active)
		{
			trail.enabled = true; 
		}
		else
		{
			trail.enabled = false; 
		}
	}

	void SetTrail()
	{
		trail = GetComponent<TrailRenderer>(); 
		if(trail)
		{
			trail.enabled =false;
		}
	}

	void Initialization()
	{
		SetTrail(); 
		holder = transform.root.gameObject.GetComponent<FightModule>(); 
	}


	public void SetForces(float light, float heavy)
	{
		LightForce = light; 
		HeavyForce = heavy; 

		Debug.Log(HeavyForce); 
	}
}
