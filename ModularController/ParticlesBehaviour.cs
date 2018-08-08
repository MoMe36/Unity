using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesBehaviour : MonoBehaviour {

	public Transform Shooter; 
	public float Speed; 
	public float Lifetime = 3f; 

	public GameObject ExplosionOnImpact; 
	public GameObject OnStartObject; 

	Vector3 Direction = Vector3.zero; 
	Quaternion Rotation = Quaternion.identity; 

	// Use this for initialization
	void Start () {

		if(OnStartObject != null)
		{
			Creation(OnStartObject);
		}
		Destroy(gameObject, Lifetime); 
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += Direction*Speed*Time.deltaTime; 
	}

	void OnTriggerEnter(Collider other)
	{
		if(ExplosionOnImpact != null)
		{
			Creation(ExplosionOnImpact); 
			// ImpactBox impact = other.gameObject.GetComponent<ImpactBox>(); 
			HitImpact impact = other.gameObject.GetComponent<HitImpact>(); 
			if(impact != null)
			{
				impact.ParticleImpact(); 
			}
			Destroy(gameObject); 
		}
	}

	public void SetShooter(Transform t)
	{
		Shooter = t; 
		Direction = Shooter.forward; 
		Rotation = Shooter.rotation; 
	}

	void Creation(GameObject target)
	{
		GameObject p = Instantiate(target, transform.position, Rotation*target.transform.rotation) as GameObject;
	}
}
