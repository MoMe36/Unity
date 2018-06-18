using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamuraiController : MonoBehaviour {

	public bool DEBUG = false; 
	public GameObject Ennemi; 
	public SamuraiFiller Filler; 
	PersoSamurai me; 

	// Use this for initialization
	void Start () {
		me = new PersoSamurai(gameObject, Filler, Camera.main); 
		me.SetEnnemi(Ennemi); 
	}
	
	// Update is called once per frame
	void Update () {
		
		me.MAJ(); 
		float x = Input.GetAxis("Horizontal"); 
		float y = Input.GetAxis("Vertical"); 

		// me.PlayerMove(x,y); 
		
		
		if(Input.GetButtonDown("XButton"))
			me.HitActivation();
		if(Input.GetButtonDown("BButton"))
			me.Dodge(x,y); 

		DebugFunc(); 

	}

	void DebugFunc()
	{
		if(DEBUG)
		{	

			DEBUG = !DEBUG; 
		}
	}
}
