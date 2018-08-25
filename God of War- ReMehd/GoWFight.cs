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

	public void Switch(HitData hit_data, bool state)
	{
		HitDict[hit_data.BoxName].Switch(hit_data, state);
		if(state)
		{
			ApplyHitImpulsion(hit_data);
		}
	}

	public void ApplyHitImpulsion(HitData hd)
	{
		rb.velocity += transform.forward*hd.ImpulsionStrength;
	}

	public void Impacted(Vector3 pos, HitData impact_hit_data)
	{
		Debug.Log("Impact called on: " + gameObject.ToString());
		Vector3 direction = Vector3.ProjectOnPlane(transform.position - pos, Vector3.up).normalized;
		Debug.DrawRay(transform.position, direction*impact_hit_data.ImpactForce, Color.red, 1f);
		rb.velocity += direction*impact_hit_data.ImpactForce;
		Debug.Log(impact_hit_data.ToString());
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
	public GoWHitImpact box;
}

[System.Serializable]
public struct HitData
{
	public string BoxName;
	public float ImpulsionStrength;
	public float ImpactForce;

	public string ToString()
	{
		return "Hit impact " + BoxName + " Impulsion: " + ImpulsionStrength.ToString() + " ImpactStrength: " + ImpactForce.ToString();
	}
}
