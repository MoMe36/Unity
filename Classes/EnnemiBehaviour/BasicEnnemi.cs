using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnnemi
{

	public Perso me; 
	public GameObject Target; 

	public GradientNavigation nav; 
	public BasicEnnemiBehaviour behav; 


	public BasicEnnemi(Perso m, GameObject t)
	{
		me = m; 
		Target = t; 
	}

	public BasicEnnemi(Perso m, GameObject t, GradientNavigation n)
	{
		me = m; 
		Target = t; 
		n = nav; 
	}

	public BasicEnnemi(Perso m, GameObject t, GradientNavigation n, BasicEnnemiBehaviour b)
	{
		me = m; 
		Target = t; 
		n = nav;
		behav = b; 
	}

	public void SetGradientNavigation(GradientNavigation n)
	{
		nav = n; 
	}

	public void SetBehaviour(BasicEnnemiBehaviour b)
	{
		behav = b; 
	}

	public void GoToEnnemi()
	{
		Vector3 v = Target.transform.position - me.transform.position; 
		v.y = 0; 
		if(v.magnitude > 5.0f)
		{
			if(nav != null)
			{
				v = nav.Navigate(me.transform, v); // Use gradient navigation if possible 
			}
			me.Move(v); 

		}
		
	}

	public string TestAction()
	{
		Vector3 v = Target.transform.position - me.transform.position; 
		string action = behav.SelectAction(v.magnitude); 

		return action; 
	}

}