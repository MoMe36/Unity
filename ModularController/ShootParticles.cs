using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootParticles : MonoBehaviour {

	public GameObject PrefabParticle; 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	public void Shoot(Transform t)
	{
		GameObject p = Instantiate(PrefabParticle, transform.position, PrefabParticle.transform.rotation) as GameObject; 
		p.GetComponent<ParticlesBehaviour>().SetShooter(t); 
	}
}
