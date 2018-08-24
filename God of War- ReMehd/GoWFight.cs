using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoWFight : MonoBehaviour {

	[Header("\t\t Hitboxes")]	
	public Boxes [] hitboxes;

	Dictionary<string, GoWHitImpact> HitDict; 
	Dictionary<string, GoWHitImpact> ImpactDict; 
	
	Animator anim; 
	Rigidbody rb; 

	// Use this for initialization
	void Start () {

		Initialization(); 
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Hit()
	{
		anim.SetTrigger("Hit"); 
	}

	void FillDicts()
	{
		HitDict = new Dictionary<string, GoWHitImpact>(); 
		ImpactDict = new Dictionary<string, GoWHitImpact>(); 
		foreach(Boxes hit in hitboxes)
		{
			if(hit.box.TypeImpact == 0)
			{
				ImpactDict.Add(hit.name, hit.box);
			}
			else if(hit.box.TypeImpact == 1)
			{
				HitDict.Add(hit.name, hit.box);
			}
		}
	}

	public void Switch(string entry, bool state)
	{
		HitDict[entry].Switch(state); 
	}

	public void ApplyHitImpulsion(HitData hd)
	{
		rb.velocity += transform.forward*hd.ImpulsionStrength; 
	}

	void Initialization()
	{
		anim = GetComponent<Animator>(); 
		rb = GetComponent<Rigidbody>(); 
		FillDicts(); 
	}

	public void Impacted(float f)
	{

	}
}

[System.Serializable]
public struct Boxes
{
	public string name; 
	public GoWHitImpact box; 
}

[System.Serializable]
public struct HitData
{
	public string BoxName; 
	public float ImpulsionStrength; 
}
