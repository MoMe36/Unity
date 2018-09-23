using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoWFight : MonoBehaviour {

	[Header("\t\t Hitboxes")]
	public Boxes [] hitboxes;

	Dictionary<string, GoWHitImpact> HitDict;
	Dictionary<string, GoWHitImpact> ImpactDict;

	public bool is_hitting;

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
			is_hitting = true;
		}
		else
		{
			is_hitting = false;
		}
	}

	public void ApplyHitImpulsion(HitData hd)
	{
		rb.velocity += transform.forward*hd.ImpulsionStrength;
	}

	public void Impacted(Transform ennemy_transform, HitData impact_hit_data)
	{
		// Vector3 direction = Vector3.ProjectOnPlane(transform.position - pos, Vector3.up).normalized;
		Vector3 direction_parameters = impact_hit_data.DirectionComponents;

		Vector3 direction = ennemy_transform.forward*direction_parameters.z +
							ennemy_transform.up*direction_parameters.y +
							ennemy_transform.right*direction_parameters.x;

		rb.velocity += direction.normalized*impact_hit_data.ImpactForce;


		anim.SetTrigger(impact_hit_data.ImpactTriggerName);
	}

	void Initialization()
	{
		anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();

		is_hitting = false;

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
	public Vector3 DirectionComponents;
	public string ImpactTriggerName;

	public string ToString()
	{
		return "Hit impact " + BoxName + " Impulsion: " + ImpulsionStrength.ToString() + " ImpactStrength: " + ImpactForce.ToString();
	}
}
