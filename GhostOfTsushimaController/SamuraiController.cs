using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamuraiController : MonoBehaviour {

	public bool DEBUG = false; 

	[Space(50)]

	[Header("\t\tTest hit frequency")]
	public float HitFreq = 0.1f; 
	public float hitcounter = 0f;
	public bool ready = true;  

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
		
		
		CountHit(); 


		if(ready)
		{
			
			// if(Input.GetButtonDown("XButton"))
			if(Input.GetKeyDown(KeyCode.E))
			{
				me.HitActivation("Hit");
				ready = false;
				hitcounter = HitFreq;
			}
			// if(Input.GetButtonDown("YButton"))
			if(Input.GetKeyDown(KeyCode.R))
			{
				me.HitActivation("HitSV");
				ready = false;
				hitcounter = HitFreq;
			}
			// if(Input.GetButtonDown("BButton"))
			if(Input.GetKeyDown(KeyCode.T))
			{
				me.HitActivation("HitSH");
				ready = false;
				hitcounter = HitFreq;
			}
		}
		// if(Input.GetButtonDown("BButton"))
		// 	me.Dodge(x,y); 

		// if(Input.GetButtonDown("YButton"))
		// 	me.ActivateDash() ;

		// DashEffect(); 
		DebugFunc(); 

	}

	void CountHit()
	{
		if(!ready)
			hitcounter -= Time.deltaTime; 
		if (hitcounter <= 0f)
			ready = true; 
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
