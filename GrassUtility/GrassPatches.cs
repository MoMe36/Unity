using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GrassPatch
{
	public Vector3 Origin; 
	public float Radius; 
	public int Number; 
}
// { 
// 	public GameObject g;
// 	[HideInInspector]
// 	public Transform place; 
// 	public float Radius; 
// 	public int StrandNumber; 

// 	List <Vector3> points = new List<Vector3>();
// 	List <Vector3> normals = new List <Vector3>(); 
// 	int [] indices; 
// 	bool Ready; 

// 	Mesh mesh; 
// 	MeshFilter filter; 
// 	MeshRenderer renderer; 


// 	public GrassPatch(GameObject G, float r,int d)
// 	{
// 		g = G; 
// 		place = g.transform; 
// 		Radius = r; 
// 		StrandNumber = d; 

// 		Ready = false; 
// 		filter = g.GetComponent<MeshFilter>(); 

// 		Debug.Log(filter);
// 	}

// 	public void Generate()
// 	{ 
// 		Debug.Log("Start Generation");
// 		Debug.Log(StrandNumber);
// 		for (int i = 0; i < StrandNumber; i++)
// 		{
// 			Debug.Log("Inside");
// 			float r = Random.Range(0.0f, Radius); 
// 			float angle = Random.Range(0.0f,360.0f); 
// 			Debug.Log("Before loop 1"); 
// 			Vector3 origin = g.transform.position + 
// 			new Vector3(r*Mathf.Cos(angle),150,r*Mathf.Sin(angle)); 
// 			Debug.Log(origin);
// 			Ray ray = new Ray(origin, -Vector3.up); 
// 			RaycastHit hit; 
// 			Debug.Log("Before loop"); 
// 			if (Physics.Raycast(ray, out hit))
// 			{
// 				Debug.Log("Hit"); 
// 				points.Add(hit.point); 
// 				indices[i] = i;
// 				normals.Add(hit.normal); 
// 			}

// 			Debug.Log(i);
// 		}

// 		Debug.Log("End Generation");
// 		mesh = new Mesh(); 
// 		mesh.SetVertices(points); 
// 		mesh.SetIndices(indices, MeshTopology.Points, 0); 
// 		mesh.SetNormals(normals); 

// 		Debug.Log(mesh);
// 		filter.mesh = mesh; 

// 		Ready = true; 
// 	}






// }