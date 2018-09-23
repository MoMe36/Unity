using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeBehaviour : MonoBehaviour {


  [Header("Position and information parameters")]
  public GoWAxeControl central_command ; 
  public Transform HandHolder; 
  public Transform BackHolder; 

  public Vector3 TransOffsetHand; 
  public Vector3 TransOffsetBack; 
  public Vector3 RotOffsetHand; 
  public Vector3 RotOffsetBack; 

  Quaternion QuatOffsetHand; 
  Quaternion QuatOffsetBack; 
	
  [Space(10)]
  [Header("Moving parameters")]
  public bool Active; 
	public float ThrowVelocity; 
	public float RecallVelocity; 
	public float StrayDistance; 
	public float StrayTime; 

	float stray_time; 
	Vector3 recall_target; 

  [Space(10)]
  [Header("Effects")]
	public GameObject ExplosionEffect; 
	public TrailRenderer [] trails; 


  public enum AxeStates {in_hand, in_back, thrown, stuck, recalled}; 
  public AxeStates current_axe_state; 
	
	Rigidbody rb;
	Animator anim; 


  // Use this for initialization
  void Start () {

  		Initiaization(); 
  }

  // Update is called once per frame
  void Update () {

  

    if(current_axe_state == AxeStates.recalled)
    {
        MoveToHand(); 
    }     


    // OffsetAdjustements(); 
  }

  void OffsetAdjustements() // Use this to adjust axe position and rotation accordingly to parents. 
  {
    RotOffset2Quat(); 
    MoveToParent(); 
    // ApplyOffsets(); 
  }
  void MoveToHand()
  {

  	transform.position = Vector3.MoveTowards(transform.position, HandHolder.position, RecallVelocity*Time.deltaTime);
  	if(Vector3.Distance(HandHolder.position, transform.position) <= 0.5f)
  	{
  		Arrival(); 
  	}
  }

  void AdjustStray()
  {
  	if (stray_time >= 0f)
  	{
  		stray_time -= Time.deltaTime;
  		recall_target = stray_time < 0f ? HandHolder.position : recall_target; 
  	}
  	else
  	{
  		recall_target = HandHolder.position; 
  	}
  }

  void SetParent(string parent)
  {
    current_axe_state = parent == "hand" ? AxeStates.in_hand : AxeStates.in_back; 
    MoveToParent(); 
  }

  void Arrival()
  {
  	anim.SetTrigger("Arrival"); 
  	SetParent("hand"); 

  	current_axe_state = AxeStates.in_hand; 

    central_command.ReceiveAxe(); 
  }

  public void Throw(Vector3 v)
  {
  	Vector3 direction = v; 
  	transform.parent = null; 
  	rb.isKinematic = false; 
  	rb.velocity += direction*ThrowVelocity; 


    current_axe_state = AxeStates.thrown; 

  	anim.SetTrigger("Shoot"); 
    Active = true; 
  }

  public void Recall()
  {
  	if(current_axe_state != AxeStates.in_hand)
  	{
      current_axe_state = AxeStates.recalled; 
	   	anim.SetTrigger("Return"); 

  		recall_target = HandHolder.transform.position; 
  	}
  }

  public void ChangeParent(string target)
  {
    if(target == "kratos")
    {

      if(current_axe_state == AxeStates.in_hand)
      {
        current_axe_state = AxeStates.in_back; 
      }
      else
      {
        current_axe_state = AxeStates.in_hand; 
      }
      MoveToParent(); 
    }
  }

  void MoveToParent()
  {

    Transform parent = current_axe_state == AxeStates.in_hand ? HandHolder : BackHolder; 
    transform.position = parent.position; 
    transform.rotation = parent.rotation; 
    transform.parent = parent; 

    ApplyOffsets(); 
  }

  void ApplyOffsets()
  {
    bool in_hand = current_axe_state == AxeStates.in_hand ? true : false;  
    Quaternion offset = in_hand ? QuatOffsetHand : QuatOffsetBack; 
    Vector3 translation_offset = in_hand ? TransOffsetHand : TransOffsetBack; 

    transform.position += (transform.root.rotation*translation_offset); 
    transform.rotation *= offset;  
  }

  void SwitchAnim(bool enabled)
  {
  	anim.enabled = enabled; 
  }

  void OnTriggerEnter(Collider other)
  {
  	if(IsThrown() || IsRecalled())
  	{
      if(other.gameObject.transform.root != HandHolder.transform.root)	
      {
        rb.isKinematic = true; 
        anim.SetTrigger("AxeImpact"); 
    
        CreateExplosion();
        Active = false; 

        current_axe_state = AxeStates.stuck; 
      }
     }
  }


  void CreateExplosion()
  {
  	GameObject explosion = Instantiate(ExplosionEffect, transform.position,  ExplosionEffect.transform.rotation) as GameObject; 
  	Destroy(explosion, 2f); 
  }


  public bool IsInHand()
  {
    return current_axe_state == AxeStates.in_hand ? true : false; 
  }

  public bool IsInBack()
  {
    return current_axe_state == AxeStates.in_back ? true : false; 
  }

  public bool IsThrown()
  {
    return current_axe_state == AxeStates.thrown ? true : false; 
  }

  public bool IsStuck()
  {
    return current_axe_state == AxeStates.stuck ? true : false; 
  }

  public bool IsRecalled()
  {
    return current_axe_state == AxeStates.recalled ? true : false; 
  }

  void RotOffset2Quat()
  {
     QuatOffsetHand = Quaternion.Euler(RotOffsetHand.x,RotOffsetHand.y, RotOffsetHand.z ); 
     QuatOffsetBack = Quaternion.Euler(RotOffsetBack.x,RotOffsetBack.y, RotOffsetBack.z ); 
  }

  void Initiaization()
  {
  	rb = GetComponent<Rigidbody>(); 
  	anim = GetComponent<Animator>(); 

  	anim.Play("SpinNoLoop", 0, 1f); 
    OffsetAdjustements();

  }




}
