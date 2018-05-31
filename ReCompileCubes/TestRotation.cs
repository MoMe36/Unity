using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotation : MonoBehaviour {

	public Mesh mesh; 
	public Material material; 
	public Collider collider; 

	public float Finesse; 

	Vector3 Increment;  


	// Use this for initialization
	void Start () {

		Increment = collider.bounds.size/Finesse; 
		
	}
	
	// Update is called once per frame
	void Update () {

		List<Matrix4x4> matrix = new List<Matrix4x4>(); 

		Vector3 sizes = collider.bounds.size; 
		for(float i = -(sizes.x-Finesse)/2 ; i< (sizes.x - Finesse)/2; i+= Finesse)
		{
			for(float j = -(sizes.y-Finesse)/2; j< (sizes.y - Finesse)/2; j+= Finesse)
			{
				for(float h = -(sizes.z-Finesse)/2; h< (sizes.z - Finesse)/2; h+= Finesse)
				{
					Vector3 pos = collider.bounds.center + transform.right*h+ transform.forward*i + transform.up*j; 
					if(collider.bounds.Contains(pos))
					{
						matrix.Add(Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one*0.7f*Finesse)); 
					}	
					
				}
			}
		}

		Debug.DrawRay(transform.position, transform.forward*5, Color.red, 1); 
		Debug.DrawRay(transform.position, transform.right*5, Color.green, 1); 
		Debug.DrawRay(transform.position, transform.up*5, Color.blue, 1); 

		Graphics.DrawMeshInstanced(mesh, 0, material, matrix); 

	}
}
