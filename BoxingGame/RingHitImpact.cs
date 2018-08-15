using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingHitImpact : MonoBehaviour {

	[Header("0 = Impact | 1 = Hit | 2 = Dodge")]	
	public int TypeImpact = 0; 
	public bool Active = false; 
	public GameObject ImpactEffect; 

	TrailRenderer trail;
	BoxerFightController holder; 


	// Use this for initialization
	void Start () {

		Initialization(); 
			
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other)
	{
		// if(TypeImpact)
		// {
		// 	RingHitImpact other_hit = other.gameObject.GetComponent<RingHitImpact>(); 
		// 	if(other_hit != null)
		// 	{
		// 		if(other_hit.Active)
		// 		{
		// 			Impacted(other.transform.position, 5); 
		// 		}
		// 	}
		// }


		if(TypeImpact == 0)
		{
			RingHitImpact other_hit = other.gameObject.GetComponent<RingHitImpact>(); 
			if(other_hit != null)
			{
				if(other_hit.TypeImpact == 1)
				{
					if(other_hit.Active)
					{
						Impacted(other.transform.position, 5); 
					}
				}
			}
		}

	}

	void Impacted(Vector3 pos, float force)
	{
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
		holder = transform.root.gameObject.GetComponent<BoxerFightController>(); 
	}


	public void SetForces(float light, float heavy)
	{
		// LightForce = light; 
		// HeavyForce = heavy; 

		// Debug.Log(HeavyForce); 
	}
}
