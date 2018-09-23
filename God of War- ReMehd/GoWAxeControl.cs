using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Good values: Rotation in back: 8.4, 77, 90
// Vector 0, 0.15, -0.4

public class GoWAxeControl : MonoBehaviour {

	[Space(10)]
	[Header("Axe references")]
	public GameObject Axe; 
	public AxeBehaviour axe_behaviour; 

	
	public float LerpSpeed; 

	Animator anim; 

	float current_layer_weight; 
	float lerp_target; 
	bool is_lerping; 
	bool is_changing; 
	bool is_ready = true;


	void Start()
	{
		Initialization(); 
	}

	void Update()
	{

	
	}

	public void ChangeAxeParent()
	{
		axe_behaviour.ChangeParent("kratos"); 
		anim.SetBool("HasAxe", axe_behaviour.IsInHand()); 
	}

	void Initialization()
	{
		anim = GetComponent<Animator>(); 
		anim.SetBool("HasAxe", axe_behaviour.IsInHand()); 
	}

	public void ChangeWeapon()
	{
		if(is_ready)
		{
			bool is_in_hand = axe_behaviour.IsInHand(); 
			bool is_in_back = axe_behaviour.IsInBack(); 
			
			if(is_in_hand || is_in_back)
			{
				anim.SetTrigger("GetWeapon"); 
				is_ready = false; 
				anim.SetLayerWeight(1, 1f); 
			}
		}
	}

	public void Throw()
	{
		if(axe_behaviour.IsInHand())
		{
			anim.SetTrigger("Throw"); 
		}
	}

	public void ReleaseAxe()
	{
		axe_behaviour.Throw(transform.forward); 
		anim.SetBool("HasAxe", false); 
	}


	public void GetAxeOver()
	{
		is_ready = true; 
		anim.SetLayerWeight(1, 0f); 
	}

	public void CallAxe()
	{
		if(!axe_behaviour.IsInHand())
		{
			anim.SetTrigger("ExpectAxe");
			lerp_target = 1f; 
			is_lerping = true; 
			anim.SetLayerWeight(1,1f); 

			axe_behaviour.Recall(); 
		}
	}

	public void ReceiveAxe()
	{
		Debug.Log("Reception called"); 
		lerp_target = 0f; 
		is_lerping = true; 
		anim.SetTrigger("ReceiveAxe"); 
		anim.SetBool("HasAxe", true); 
		anim.SetLayerWeight(1,0f); 
	}


}


// [System.Serializable]
// public struct BakedPosition
// {
// 	public Vector3 PosOffset; 
// 	public Vector3 EulerRotOffset; 
// 	[HideInInspector] public Quaternion RotOffset; 

// }
