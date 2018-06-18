using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ImpulsionHolder 
{

	public DelayedImpulsion impulsion; 
	public string Correspondance = "Name"; 

	int max_enumerator = 0; 
	int enumerator = 0; 
	bool ready = false; 

	bool active = false; 

	public ImpulsionHolder()
	{

	}

	public bool Analyze(CharacterStates c)
	{
		bool result = false; 
		if(c.Name == Correspondance)
		{
			result = SwitchOn(); 
		}
		else
		{
			SwitchOff(); 
		}
		
		return result; 
	}

	bool SwitchOn()
	{
		if(!active)
		{
			active = true; 
			return true; 
		}
		else
		{
			return false; 
		}
	}

	void SwitchOff()
	{
		active = false; 
	}

	public DelayedImpulsion GetClone()
	{
		DelayedImpulsion i = new DelayedImpulsion(impulsion.Direction,impulsion.Strengh,impulsion.Delay,impulsion.Duration); 
		return i ; 
	}

}
