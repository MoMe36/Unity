using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class BoxerMoveController : MonoBehaviour {

	// public Text debug_text; 

	public Transform Ennemy; 
	public float Speed = 1f; 
	public float RotatingSpeed = 1f; 
	public float LerpAnimSpeed = 1f; 

	Vector3 CurrentDirection = Vector3.zero; 
	Vector3 DashDirection; 
	Vector2 AnimDirection; 
	Animator anim;
	Rigidbody rb; 
	Camera cam; 

	float height; 
	
	// Use this for initialization
	void Start () {

		Initialization(); 
	}
	
	// Update is called once per frame
	void Update () {


		RotateTowardsEnnemy(); 
		AdjustAnim(); 

		// DebugVision(); 
		
	}


	// void DebugVision()
	// {
	// 	Vector2 d = GetComponent<InputsBoxer>().GetDirection(); 
	// 	Vector3 d_adapted = CamToPlayer(d); 
	// 	float angle = Mathf.Deg2Rad*Vector3.SignedAngle(transform.forward, d_adapted, transform.up); 

	// 	float x_angle = Mathf.Cos(angle); 
	// 	float y_angle = Mathf.Sin(angle); 

	// 	Debug.DrawRay(transform.position + transform.up, 5*d_adapted, Color.red, 1f); 
	// 	Vector3 anim_vec = Quaternion.AngleAxis(angle, transform.up)*transform.forward; 
	// 	string s = "Angle with character: " + angle.ToString(); 
	// 	s += "\nFrom angle - X" + x_angle.ToString() +" Y " +y_angle.ToString(); 
	// 	s += "\nAnimDirection " + AnimDirection.ToString(); 

	// 	debug_text.text = s; 
	// }

	void RotateTowardsEnnemy()
	{
		Vector3 to_ennemy = Vector3.ProjectOnPlane(Ennemy.position - transform.position, Vector3.up); 
		Quaternion ideal_rotation = Quaternion.LookRotation(to_ennemy, Vector3.up);
		transform.rotation =  Quaternion.Slerp(transform.rotation, ideal_rotation, RotatingSpeed*Time.deltaTime); 
	}

	public void PlayerMove(Vector2 direction, bool dash)
	{
		if(dash)
		{
			DashDirection = direction.magnitude > 0.2f ? CamToPlayer(direction) : transform.forward; 
			float angle = Mathf.Deg2Rad*Vector3.SignedAngle(transform.forward, DashDirection, Vector3.up); 
			AnimDirection = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle)); 
			Dash(); 
		}
	}

	void AdjustAnim()
	{
		Vector2 anim_xy = new Vector2(anim.GetFloat("X"), anim.GetFloat("Y")); 
		anim_xy = Vector2.Lerp(anim_xy, AnimDirection, LerpAnimSpeed*Time.deltaTime);
		
		anim.SetFloat("X", anim_xy.x); 
		anim.SetFloat("Y", anim_xy.y); 
	}

	public void ApplyDashForce()
	{
		rb.velocity += DashDirection*Speed; 
	}

	Vector3 CamToPlayer(Vector2 direction)
	{
		Vector3 forward = Vector3.ProjectOnPlane(transform.position - cam.transform.position, Vector3.up); 
		Vector3 right = Quaternion.AngleAxis(90, Vector3.up)*forward; 

		return (forward*direction.y + right*direction.x).normalized; 
	}

	void Dash()
	{
		anim.SetTrigger("Dash"); 
	}

	void Initialization()
	{
		cam = Camera.main; 
		rb = GetComponent<Rigidbody>(); 
		anim = GetComponent<Animator>(); 

		
	}
}
