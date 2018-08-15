using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxerFightController : MonoBehaviour {

	public float ImpulsionStrength = 1f; 
	public RingNamedHitbox [] Hitboxes; 
	[HideInInspector] public bool Ready; 
	Animator anim;
	Rigidbody rb; 

	Dictionary <string, RingHitImpact> hit_dict; 
	Dictionary <string, RingHitImpact> impact_dict; 
	Dictionary <string, RingHitImpact> dodge_dict; 

	// Use this for initialization
	void Start () {

		FillDicts(); 
		anim = GetComponent<Animator>(); 
		rb = GetComponent<Rigidbody>(); 
		Ready = true; 
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Act(bool Left, bool Right, bool Direct, bool Hook, bool Upper, bool Dodge)
	{
		if(Ready)
		{
			string side = Right ? "Right" : "Left"; 
			if(Direct)
			{
				LaunchHit(side, "Direct"); 
			}
			else if(Hook)
			{
				LaunchHit(side, "Hook"); 
			}
			else if(Upper)
			{
				LaunchHit(side, "Upper"); 
			}
			else if(Dodge)
			{
				LaunchHit(side, "Dodge"); 
			}
		}
	}
	public void Impulse()
	{
		rb.velocity += transform.forward*ImpulsionStrength; 
	}

	public void Impacted(float force)
	{
		anim.SetTrigger("Impact");
	}

	void LaunchHit(string side, string hit)
	{
		Debug.Log(side + " " +  hit); 
		anim.SetBool(side, true); 
		anim.SetTrigger(hit); 
		Ready = false; 
	}

	public void BeReady()
	{
		Ready = true; 
	}

	public void Switch(string entry, bool state, bool dodge)
	{
		if(dodge)
			dodge_dict[entry].Switch(state); 
		else
			hit_dict[entry].Switch(state); 
	}

	void FillDicts()
	{
		hit_dict = new Dictionary<string, RingHitImpact>(); 
		impact_dict = new Dictionary<string, RingHitImpact>(); 
		dodge_dict = new Dictionary<string, RingHitImpact>(); 
		foreach(RingNamedHitbox hit in Hitboxes)
		{
			if(hit.Box.TypeImpact == 0)
			{
				impact_dict.Add(hit.Name, hit.Box);
			}
			else if(hit.Box.TypeImpact == 1)
			{
				hit_dict.Add(hit.Name, hit.Box);
				// hit.box.SetForces(LightForce, HeavyForce); 
			}
			else
			{
				dodge_dict.Add(hit.Name, hit.Box); 
			}
		}
	}
}

[System.Serializable]
public struct RingNamedHitbox
{
	public string Name; 
	public RingHitImpact Box; 
}