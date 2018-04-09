using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class BasicEnnemiControl : MonoBehaviour {

	Perso me_perso; 
	public PersoFiller data; 
	public BasicEnnemi me; 
	public GameObject Target; 

	public GradientNavigation nav; 

	public bool Move;
	public bool Act = false;  

	// Use this for initialization
	void Start () {

		me_perso = new Perso(gameObject,gameObject,data);
		me = new BasicEnnemi(me_perso, Target); 

		me.SetGradientNavigation(nav);
		me.SetBehaviour(GetComponent<BasicEnnemiBehaviourHolder>().Behaviour); 
		
		
	}
	
	// Update is called once per frame
	void Update () {


		if(Move)
			me.GoToEnnemi();

		if(Act)
		{
			Act = false; 
			string s = me.TestAction(); 
			Debug.Log(s); 
		}




	}


	// void OnDrawGizmos()
	// {
	// 	Gizmos.color = Color.green; 

	// 	Vector3 [] liste = nav.DebugVec(); 
	// 	foreach (Vector3 v in liste)
	// 	{
	// 		Gizmos.DrawSphere(transform.position - v, 0.5f); 
	// 	}
	// }
}
