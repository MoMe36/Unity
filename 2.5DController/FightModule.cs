using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class FightModule : MonoBehaviour {

	[Header("\t\t Hit Parameters")]
	public float LightForce; 
	public float HeavyForce; 


	[Header("\t\t Hitboxes")]	
	public Boxes [] hitboxes;

	[Header("\t\t Shoot Parameters")]
	public ShootParticles RayShooter;
	
	Animator anim; 
	Rigidbody rb; 
	Dictionary <string, HitImpact> HitDict; 
	Dictionary <string, HitImpact> ImpactDict;

	// Use this for initialization
	void Start () {
		
		Initialization();
		

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Hit(bool hit, bool heavy_hit)
	{
		if(hit)
		{
			anim.SetBool("Hit", true); 
		}
		else if(heavy_hit)
		{
			anim.SetBool("HitHeavy", true); 
		}
	}

	public void Impacted(float force)
	{
		anim.SetTrigger("Impact");
		rb.AddForce(-transform.forward*force); 
	}

	public void Shoot()
	{
		anim.SetTrigger("Shoot"); 
	}

	public void ShootAction()
	{
		RayShooter.Shoot(transform); 
	}

	void FillDicts()
	{
		HitDict = new Dictionary<string, HitImpact>(); 
		ImpactDict = new Dictionary<string, HitImpact>(); 
		foreach(Boxes hit in hitboxes)
		{
			if(hit.box.TypeImpact)
			{
				ImpactDict.Add(hit.name, hit.box);
			}
			else
			{
				HitDict.Add(hit.name, hit.box);
				hit.box.SetForces(LightForce, HeavyForce); 
			}
		}
	}

	public void Switch(string entry, bool state)
	{
		HitDict[entry].Switch(state); 
	}

	void Initialization()
	{
		anim = GetComponent<Animator>(); 
		rb = GetComponent<Rigidbody>(); 
		FillDicts(); 
	}
}

[System.Serializable]
public struct Boxes
{
	public string name; 
	public HitImpact box; 
}
