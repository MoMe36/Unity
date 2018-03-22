using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityMap 
{
	[HideInInspector]
	public Vector3 Center; 
	public int Size;
	public float Space; 
	public LayerMask NotWalkable;
	[HideInInspector]
	public UtilityNode [] nodes; 

	int iterator = 0; 
	Vector3 Offset; 
	float MaxMagnitude; 

	public UtilityMap(Vector3 v, int n, float f, LayerMask l)
	{
		Center = v; 
		Size = n; 
		Space = f; 
		NotWalkable = l; 

		float fsize = (float)Size; 
		MaxMagnitude = fsize*1.41f;
		nodes = new UtilityNode [Size*Size]; 
		Offset = new Vector3(-(fsize-1)/2.0f*Space,0,-(fsize-1)/2.0f*Space); 

		Setup(); 
	}

	void Setup()
	{	
		

		int counter = 0; 
		for (int i = 0; i<Size; i++)
		{
			for (int j = 0; j<Size; j++)
			{
				Vector3 node_offset = new Vector3(i*Space,0,j*Space) + Offset;
				Vector3 pos = Center + node_offset; 
				UtilityNode node = new UtilityNode(pos, Space, i,j, node_offset.magnitude/MaxMagnitude, NotWalkable); 
				nodes[counter] = node; 
				counter ++; 
			}
		}
	}

	public void UpdateNodesPosition()
	{
		nodes[iterator].UpdatePosition(Center, Space, Offset); 
	}

	public void UpdateNodesValues()
	{
		float d = nodes[iterator].FromCenter((float)Size/2.0f);
		nodes[iterator].UpdateValue();
	}

	public void UpdateCenter(Vector3 v)
	{
		Center.x = v.x; 
		Center.z = v.z; 
	}

	public Vector3 GetBestPos()
	{
		float minCost = nodes[0].Value; 
		int pos = 0; 
		for (int i = 0; i<nodes.Length; i++)
		{
			if(nodes[i].Value < minCost)
			{
				minCost = nodes[i].Value; 
				pos = i; 
			}
		}

		return nodes[pos].Position; 
	}

	public void Update(Vector3 v)
	{
		UpdateCenter(v); 
		UpdateNodesPosition(); 
		UpdateNodesValues();

		iterator = (iterator+1)%(Size*Size); 
	}

	public void SetWeightFactor(float a)
	{
		foreach(UtilityNode n in nodes)
		{
			n.SetWeightFactor(a);
		}
	}





}
