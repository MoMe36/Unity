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

	public CharacterStates(string n, List<string> s, bool p, bool c)
	{
		Name = n; 
		Correspondances = s; 
		Passive = p;
		Current = c; 
	}
}
