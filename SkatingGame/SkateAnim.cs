using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkateAnim : MonoBehaviour {

	public float AnimationLerpSpeed; 
	SkateControl control; 
	InputProcessing inputs; 
	Animator anim; 


	[HideInInspector] public Quaternion FromInputs; 
	float Tilt; 
	bool LastAerial = false; 
	// Use this for initialization
	void Start () {
		Initialization(); 
	}
	
	// Update is called once per frame
	void Update () {
		
		ManageTilt(); 
		ManageAir(); 
	}

	void ManageAir()
	{
		bool current_aerial = control.aerial; 
		if(current_aerial)
		{
			if(!LastAerial)
			{
				LastAerial = true; 
				anim.SetTrigger("Aerial"); 
			}
		}
		else
		{
			if(LastAerial)
			{
				LastAerial = false; 
				anim.SetTrigger("Land"); 
			}
		}
	}

	void ManageTilt()
	{
		Vector3 expected_direction = control.VelocityRotation*transform.forward; 
		float angle = Vector3.SignedAngle(transform.forward, expected_direction, transform.up); 
		// FromInputs = control.VelocityRotation; 
		// // float angle = FromInputs.eulerAngles.y - 360; 
		Debug.Log(angle);
		 // angle = Mathf.Clamp(Vector3.SignedAngle(forward, adapted_direction, Vector3.up), -90,90); 
		angle = Mathf.Clamp(angle*3f, -5f,5f); 
		Tilt = (angle+5f)/10f; 
		

		AdjustAnim(); 
	}	

	void AdjustAnim()
	{
		float tilt = anim.GetFloat("Tilt"); 
		tilt = Mathf.Lerp(tilt, Tilt, AnimationLerpSpeed*Time.deltaTime); 
		anim.SetFloat("Tilt", tilt); 
	}

	void Initialization()
	{
		control = GetComponent<SkateControl>(); 
		anim = GetComponent<Animator>(); 

		Tilt = 0.5f; 
		anim.SetFloat("Tilt", Tilt); 
	}


}
