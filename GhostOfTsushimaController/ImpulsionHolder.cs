using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ImpulsionHolder 
{

	public DelayedImpulsion impulsion; 
	public string Correspondance = "Name"; 

	public List<string> Correspondances = new List<string>(); 
	string current_name = ""; 

	int max_enumerator = 0; 
	int enumerator = 0; 
	bool ready = false; 

	bool active = false; 

	public ImpulsionHolder()
	{

	}

	void Init()
	{
		ready = true; 
		current_name = Correspondances[0]; 
	}

	public bool Analyze(string c)
	{
		if(!ready)
			Init(); 

		bool result = false; 
		if(Correspondances.Contains(c))
		{
			result = SwitchOn(c); 
			Debug.Log(c); 
		}
		else	
		{
			SwitchOff(); 
		}
		
		return result; 
	}

	bool SwitchOn(string s)
	{
		if(!active)
		{
			active = true; 
			return true; 
		}
		else
		{
			if(current_name == s)
				return false; 
			else
			{
				current_name = s; 
				return true; 
			}
		}
	}

	void SwitchOff()
	{
		active = false; 
	}

	public DelayedImpulsion GetClone(Transform t)
	{
		Vector3 v = t.forward*impulsion.Direction.z + t.right*impulsion.Direction.x + t.up*impulsion.Direction.y; 
		DelayedImpulsion i = new DelayedImpulsion(v.normalized,impulsion.Strengh,impulsion.Delay,impulsion.Duration); 
		return i ; 
	}

}
