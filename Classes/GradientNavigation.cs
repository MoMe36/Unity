using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// -----------------------------------------------

	// TO USE THIS CLASS 
	// Using this class implies having some way of moving and rotation the 
	// submitted character. This class only computes a direction toward which the character should 
	// go in order to reach its target and staying away from walls. 


// -----------------------------------------------


[System.Serializable]
public class GradientNavigation
{
	public float MaxDistance = 15; 
	public int MaxObstacles = 10; 
	public float AngleIncrement = 5; 
	public float MaxAngle = 60; 
	public float ForgetTime = 1.0f;  

	public float Repulsion = 1.0f; 


	int iterator = 0; 

	float angle = 0; 
	Vector3 [] Obstacles; 
	float [] TimeToForget; 


	public GradientNavigation()
	{
		Obstacles = new Vector3 [MaxObstacles]; 
		TimeToForget = new float [MaxObstacles]; 	
	}


	public void Look(Transform transform)
	{
		Quaternion rot = Quaternion.AngleAxis(angle, transform.up); 
		Vector3 direction = rot*transform.forward; 

		Ray ray = new Ray(transform.position, direction); 
		RaycastHit hit; 



		if(Physics.Raycast(ray, out hit, MaxDistance))
		{
			Obstacles[iterator] = transform.position - hit.point; 
			TimeToForget[iterator] = ForgetTime; 
			// Debug.DrawRay(ray.origin, ray.direction*(transform.position - hit.point).magnitude, Color.red,1.0f); 
		}
		// else
		// {
		// 	Debug.DrawRay(ray.origin, ray.direction*MaxDistance, Color.red,1.0f); 
		// }

		iterator = (iterator + 1)%MaxObstacles; 
		angle += AngleIncrement; 
		angle = (angle > MaxAngle) ? -MaxAngle+Random.Range(1,7) : angle; 
	}

	public void Forget()
	{
		TimeToForget[iterator] -= Time.deltaTime*MaxObstacles; 
		if(TimeToForget[iterator] <= 0.0f)

			Obstacles[iterator] = Vector3.zero; 
	}

	public Vector3 ComputeDirection(Vector3 direction)
	{
		Vector3 somme = Vector3.zero; 
		foreach(Vector3 v in Obstacles)
		{
			if( v.magnitude > 0)
				somme += v.normalized/v.magnitude; 
		}
 
		return somme; 
	}

	public Vector3 Navigate(Transform transform, Vector3 dir)
	{
		Look(transform); 
		Forget(); 
		Vector3 v = ComputeDirection(dir); 

		// Debug.DrawRay(transform.position, v*5, Color.red, 1.0f); 
		// Debug.DrawRay(transform.position, dir.normalized*5, Color.green, 1.0f); 

		Vector3 sum = Repulsion*v + dir; 
		return sum.normalized; 
	}


	public Vector3 [] DebugVec()
	{
		return Obstacles; 
	}









}