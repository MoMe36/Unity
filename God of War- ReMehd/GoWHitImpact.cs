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

	[HideInInspector] public HitData current_hit_data;

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
						Impacted(other_hit, other_hit.current_hit_data);
					}
				}
			}
		}

	}

	void Impacted(GoWHitImpact other_hit, HitData impact_hit_data)
	{
		// Stopping Time 
		GoWCommon.StopTime(); 

		// Visual FX
		Vector3 hit_position = other_hit.transform.position;
		Vector3 ennemy_position = other_hit.holder.transform.position;
		Transform ennemy_transform = other_hit.holder.transform;
		GameObject p = Instantiate(ImpactEffect, hit_position, ImpactEffect.transform.rotation) as GameObject;

		// Physical response and animation
		holder.Impacted(ennemy_transform, impact_hit_data);
	}

	public void Switch(HitData hit_data, bool state)
	{
		Active = state;
		if(Active)
		{
			current_hit_data = hit_data;
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
