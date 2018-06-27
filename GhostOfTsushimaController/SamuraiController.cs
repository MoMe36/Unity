using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamuraiController : MonoBehaviour {

	public bool DEBUG = false; 

	[Space(50)]
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

		me.PlayerMove(x,y); 
		
			// if(Input.GetButtonDown("XButton"))
		if(Input.GetKeyDown(KeyCode.E))
		{
			me.HitActivation("Hit");
		}
		// if(Input.GetButtonDown("YButton"))
		if(Input.GetKeyDown(KeyCode.R))
		{
			me.HitActivation("HitSV");
		}
		// if(Input.GetButtonDown("BButton"))
		if(Input.GetKeyDown(KeyCode.T))
		{
			me.HitActivation("HitSH");
		}
		// }
		// if(Input.GetButtonDown("BButton"))
		// 	me.Dodge(x,y); 

		// if(Input.GetButtonDown("YButton"))
		// 	me.ActivateDash() ;

		// DashEffect(); 
		DebugFunc(); 

	}

	void DebugFunc()
	{
		if(DEBUG)
		{	
			DEBUG = !DEBUG; 
			me = new PersoSamurai(gameObject, Filler, Camera.main); 
		}
	}
}
