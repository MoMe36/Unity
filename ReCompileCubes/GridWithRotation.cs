using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridWithRotation : MonoBehaviour {

	public Vector3 Offset; 

	public Vector3 Limits; 
	public float Finesse = 1f; 

	public float TemporaryLimit; 

	public Collider collider; 
	public Mesh mesh; 
	public Material growing_material;
	public Material normal_material;
	public Material shrinking_material; 

	public int MaxCount = 50; 

	public bool DrawGizmos = false; 


	List<Matrix4x4> GrowingCubes = new List<Matrix4x4>(); 
	List<Matrix4x4> ShrinkingCubes = new List<Matrix4x4>(); 
	List<Matrix4x4> NormalCubes = new List<Matrix4x4>(); 
	Vector3 Position; 
	Quaternion Rotation; 

	float MaxSize; 


	void Start () {

		Position = collider.bounds.center; 
		Rotation = collider.transform.rotation; 
		MaxSize = 0.7f*Finesse; 
		Create(); 
		
	}
	
	// Update is called once per frame
	void Update () {


		if((Position - collider.bounds.center).magnitude > Finesse)
		{
			GetNew(); 
			Discard(); 
			Position = collider.bounds.center; 
		}
		else if(Rotation != collider.transform.rotation)
		{
			Rotation=  collider.transform.rotation;
			GetNew(); 
			Discard(); 
		}
		else
		{
			Scale(); 
		}


		// List<Matrix4x4> full = new List<Matrix4x4>(); 
		// full.AddRange(GrowingCubes);
		// full.AddRange(ShrinkingCubes); 
		// full.AddRange(NormalCubes);  //.Add(ShrinkingCubes).ToList(); 

		Graphics.DrawMeshInstanced(mesh,0,growing_material,GrowingCubes);
		Graphics.DrawMeshInstanced(mesh,0,normal_material,NormalCubes);
		Graphics.DrawMeshInstanced(mesh,0,shrinking_material,ShrinkingCubes);

		
	}


	void GetNew() // Il faut juste checker les bords, et le cas échéant les aujouter à Growing
	{
		int [] iterator = new int []{-1,1}; 
		// for(float i = 0; i<2; i+= 1)
		// {
		// 	for(float j = 0; j<2; j+= 1)
		// 	{
		// 		for(float h = 0; h<2;h+= 1)
		// 		{

		for(float i = -Limits.x/2f; i<Limits.x/2; i+= Finesse)
		{
			for(float j = -Limits.y/2f; j<Limits.y/2; j+= Finesse)
			{
				for(float h = -Limits.z/2f; h<Limits.z/2;h+= Finesse)
				{
					if(GrowingCubes.Count < MaxCount)
					{
						Vector3 pos = collider.bounds.center + transform.right*h+ transform.forward*i + transform.up*j; 
						if(collider.bounds.Contains(pos))
						{
							Matrix4x4 m = Matrix4x4.TRS(pos, Quaternion.identity,Vector3.one*Random.Range(0.01f,0.1f)); 
							GrowingCubes.Add(m); 
						}
					}
				}
			}
		}
	}

	void Discard()
	{
		List<Matrix4x4> Tampon = new List<Matrix4x4>(); 

		foreach(Matrix4x4 m in NormalCubes)
		{
			Vector3 pos = m.GetColumn(3);
			if(!collider.bounds.Contains(pos))
			{
				if(ShrinkingCubes.Count < MaxCount)
					ShrinkingCubes.Add(m); 
			}
			else
			{
				if(Tampon.Count < MaxCount)
					Tampon.Add(m); 
			}
		}
		NormalCubes = Tampon; 

	}

	void Scale()
	{
		// string s = "Normal: " + NormalCubes.Count.ToString() + " Growing: " + GrowingCubes.Count.ToString() + " Shrinking: " + ShrinkingCubes.Count.ToString(); 
		// Debug.Log(s); 
		List<Matrix4x4> ToBeRemoved = new List<Matrix4x4>(); 
		int counter = 0; 

		foreach(Matrix4x4 m in GrowingCubes)
		{
			Vector3 pos = m.GetColumn(3);
			if(collider.bounds.Contains(pos))
			{
				float scale = m[0,0];
				if (scale >= MaxSize)
				{
					// GrowingCubes.Remove(m); 
					// Debug.Log("Adding to normal"); 
					NormalCubes.Add(m); 
					ToBeRemoved.Add(m); 
				} 
				else
				{
					string ss = "Scale before: " + scale.ToString(); 
					scale += Time.deltaTime; 
					ss += " Scale after: " + scale.ToString(); 
					Matrix4x4 new_matrix = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one*scale); 
					GrowingCubes[counter] = new_matrix;
					
				}
			}
			else
			{
				ToBeRemoved.Add(m); 
				// GrowingCubes.Remove(m); 
				ShrinkingCubes.Add(m); 
			}

			counter += 1; 
		}

		

		counter = 0; 

		foreach(Matrix4x4 m in ShrinkingCubes)
		{
			float scale = m[0,0]; 
			scale -= Time.deltaTime; 
			if(scale >= 0.05)
			{
				Matrix4x4 new_matrix = Matrix4x4.TRS(m.GetColumn(3), Quaternion.identity, Vector3.one*scale);
				ShrinkingCubes[counter] = new_matrix; 
			}
			else
			{
				ToBeRemoved.Add(m); 
				// ShrinkingCubes.Remove(m); 
			}
			counter += 1; 
		}

		foreach(Matrix4x4 m in ToBeRemoved)
		{
			if (ShrinkingCubes.Contains(m))
			{
				ShrinkingCubes.Remove(m); 
			}
			// if (NormalCubes.Contains(m))
			// {
			// 	NormalCubes.Remove(m); 
			// }
			if (GrowingCubes.Contains(m))
			{
				GrowingCubes.Remove(m); 
			}
		}

		ToBeRemoved.Clear();
		// Put new cubes in GrowingCubes

		// Put cubes big enough in NormalCubes

		// Check for too far cubes and put them in ShrinkingCubes

		// Destroy smallest cubes
	}

	void Create()
	{
		GrowingCubes.Clear(); 
		for(float i = -Limits.x/2f; i<Limits.x/2; i+= Finesse)
		{
			for(float j = -Limits.y/2f; j<Limits.y/2; j+= Finesse)
			{
				for(float h = -Limits.z/2f; h<Limits.z/2;h+= Finesse)
				{
					Vector3 pos = Offset + transform.right*h+ transform.forward*i + transform.up*j; 
					// pos = transform.rotation*pos; 
					if(collider.bounds.Contains(pos))
					{
						float scale = ((pos - collider.bounds.center).magnitude < TemporaryLimit) ? MaxSize : Random.Range(0.01f,0.3f); 
						Matrix4x4 m = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one*scale); 
						GrowingCubes.Add(m); 
					}
					
				}
			}
		} 
	}


}
