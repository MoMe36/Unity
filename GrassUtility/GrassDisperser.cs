using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GrassDisperser : MonoBehaviour {

	public MeshFilter filter; 
	public float HeightOffset; 
	public List <GrassPatch> grass = new List <GrassPatch>(); 

	Mesh mesh; 

	// Use this for initialization
	void Start () {

		FillTerrain(); 
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FillTerrain()
	{ 
		int maxStrands = 0; 
		for (int i = 0; i<grass.Count; i++)
		{
			maxStrands += grass[i].Number; 
		}
      

       	List<Vector3> positions = new List<Vector3>();
        int[] indicies = new int[maxStrands];
        List<Color> colors = new List<Color>();
        List<Vector3> normals = new List<Vector3>();

        int indices_counter = 0;

		for(int i = 0; i<grass.Count; i++)
		{
			Vector3 origin = grass[i].Origin; 
			for (int j = 0; j<grass[i].Number; j++)
			{
				float r = Random.Range(0.0f,grass[i].Radius); 
				float angle = Random.Range(0.0f,360.0f); 

				Vector3 pos = origin + new Vector3(r*Mathf.Cos(angle),0,r*Mathf.Sin(angle)); 
				Ray ray = new Ray(pos, - transform.up); 
				RaycastHit hit; 

				if (Physics.Raycast(ray, out hit))
				{

					pos = hit.point + new Vector3(0,HeightOffset,0); 
					positions.Add(pos); 
					normals.Add(hit.normal); 
					indicies[indices_counter] = indices_counter; 
					indices_counter += 1; 
				}
			}
		}
		mesh = new Mesh();
        mesh.SetVertices(positions);
        mesh.SetIndices(indicies, MeshTopology.Points, 0);
        // mesh.SetColors(colors);
        mesh.SetNormals(normals);
        filter.mesh = mesh;

	}


	void OnDrawGizmos()
	{
		Gizmos.color = Color.red; 
		for (int i = 0; i<grass.Count; i++)
		{
			Gizmos.DrawCube(grass[i].Origin, Vector3.one); 
			Gizmos.DrawWireSphere(grass[i].Origin, grass[i].Radius); 
		}
	}
}
