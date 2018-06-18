using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterStates
{
	public string Name; 
	public List<string> Correspondances = new List<string>(); 
	public bool Passive; 
	[HideInInspector]
	public bool Current; 
	// [HideInInspector]
	// public bool was_changed = false; 
	// [HideInInspector]
	public bool FightingState = false; 

	public CharacterStates(string n, List<string> s, bool p, bool c)
	{
		Name = n; 
		Correspondances = s; 
		Passive = p;
		Current = c; 
	}

	public void UnlockPassive()
	{
		Passive = false; 
		// Debug.Log("Unlocking " + Name); 
		// was_changed = true; 
	}

	public void Reset()
	{
		// if(was_changed)
		// {
			// was_changed = false; 
			Passive = true; 
		// }
	}
}
