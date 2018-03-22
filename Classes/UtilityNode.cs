using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityNode 
{
	public Vector3 Position; 
	public float Value; 
	public int index_x; 
	public int index_y;  

	public float Space; 

	public float distance_cost = 0; 

	public LayerMask NotWalkable; 


	float factor; 
	float alpha; 

	public UtilityNode(Vector3 v, float s, int x, int y, float val, LayerMask l)
	{
		Position = v; 
		Value = val; 
		index_x = x; 
		index_y = y; 
		Space = s; 

		distance_cost = val; 
		NotWalkable = l; 
	}

	public float FromCenter(float half_s)
	{
		float dx =((float)index_x-half_s + 0.5f); 
		float dy =((float)index_y-half_s + 0.5f);
		return (dx*dx + dy*dy);
	}

	public void UpdatePosition(Vector3 center, float space, Vector3 o)
	{
		Position = center + new Vector3(index_x*space, 0,space*index_y) + o;
	}

	public void UpdateValue()
	{
		Collider [] statics = Physics.OverlapSphere(Position, Space/2.0f, NotWalkable); 
		if (statics.Length > 0)
		{
			Value = 1; 
		}
		else if (!Physics.Raycast(Position, Vector3.down, 2*Space))
		{
			Value = 1.0f; 
		}
		else
		{
			Value = distance_cost*alpha + EnnemiCost()*(1-alpha);
			// Value = EnnemiCost();
		}
	}

	public float EnnemiCost()
	{
		Collider [] Ennemies = Physics.OverlapSphere(Position, 4*Space);
		if (Ennemies.Length > 0)
		{
			float counter = 0; 
			for(int i = 0; i<Ennemies.Length; i++)
			{

				if(Ennemies[i].GetComponent<EnnemiControl>())
				{
					float d = 1-(Ennemies[i].transform.position - Position).magnitude/(factor*4*Space);
					d = Mathf.Clamp01(d); 
					// float d = 1-((Ennemies[i].transform.position - Position).magnitude/(4*Space)); 
					counter += d; 
				}

			}
			return counter; 
		}
		return 0.0f; 
	}

	public void SetWeightFactor(float a)
	{
		alpha = a; 
	}






}