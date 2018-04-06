using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehav : MonoBehaviour {

	public float Life = 1;
	[HideInInspector] 
	public Vector3 direction = Vector3.one; 
	public float Speed = 2 ; 

	// Use this for initialization
	void Start () {

		Destroy(gameObject, Life); 
		
	}
	
	// Update is called once per frame
	void Update () {

		transform.position += direction*Time.deltaTime*Speed;
		
	}

	public void SetDirection(Vector3 v)
	{
		direction = v; 
	}

	void OnTriggerEnter(Collider other)
	{
		GameObject g = other.gameObject; 
		Rigidbody rb = g.GetComponent<Rigidbody>(); 
		if (rb)
		{
			rb.AddForce((g.transform.position - transform.position)*1000); 
		}

		Destroy(gameObject); 
	}
}
