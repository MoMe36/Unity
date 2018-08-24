using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoWHitImpact : MonoBehaviour {

	[Header("0 = Impact | 1 = Hit")]	
	public int TypeImpact = 0; 
	public bool Active = false; 
	public GameObject ImpactEffect; 

	TrailRenderer trail; 
	GoWFight holder; 	

	// Use this for initialization
	void Start () {
		Initialization(); 
			
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other)
	{
		if(TypeImpact == 0)
		{
			GoWHitImpact other_hit = other.gameObject.GetComponent<GoWHitImpact>(); 
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

	void Initialization()
	{
		// SetTrail(); 
		trail = GetComponent<TrailRenderer>(); 
		trail.enabled = false; 
		holder = transform.root.gameObject.GetComponent<GoWFight>(); 
	}
}
