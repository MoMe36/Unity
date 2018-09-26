using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BaseMove : MonoBehaviour {

  [Space(10)]
  [Header("Basics")]

  public float Speed = 1f;
  public float RotatingSpeed = 1f;
  public float JumpForce = 1f;
  public float AdditionalGravity = 3f;
  public float LerpGravitySpeed = 1f;
  public float LerpAnimSpeed = 1f;
  float current_additional_gravity = 0f;


  [Space(10)]
  [Header("MinStepMax")]
  public float MaxSpeed = 15f;
  public float MaxSpeedJumping = 15f;
  public float MaxSpeedLerp = 1f;
  public float LandingAccelerationRatio = 1f;
  // public LerpVector LerpDragSpeed;
  public BaseImmediateChange DragStates;

  [Space(10)]
  [Header("Dash parameters")]
  public float DashForce = 1f;
  public float DashTime = 5f; 
  public TrailRenderer [] DashTrails;
  public GameObject DashExplosionEffect;
  public Transform DashExplosionPosition;
  [HideInInspector] public bool is_dashing = false;


  [Space(10)]
  [Header("Jump Control data")]
  public float MaxHeight = 2f;
  public float JumpControlLerpSpeed = 1f;
  public string JumpControlAnimationName = "Jump";
  public float InitialJumpLerpValue = 0.5f;
  [HideInInspector] public bool is_jumping;
  float jump_lerp_current_value = 0.5f;


  public enum BaseStates {move, high_speed, dash, prepare_throw, other}; 
  [HideInInspector] public BaseStates CurrentBaseState = BaseStates.move;  

  bool is_moving = false;
  bool has_input = false;
  Vector3 CurrentDirection = Vector3.zero;
  Vector3 CurrentDirectionCam =Vector3.zero;

  Quaternion TargetRotation;
  float TargetAngle;

  Animator anim;
  Rigidbody rb;
  Camera cam;
  BaseFight fight;


  float height;



  // Use this for initialization
  void Start () {

    Initialization();

  }

  // Update is called once per frame
  void Update () {

    AdjustRotation();
    AdjustPhysics();
    AdjustAnim();

  }

  void AdjustPhysics()
  {
    AdjustDrag();
    CheckGroundContact();
    ApplyAdditionalGravity();
  }

  void AdjustRotation()
  {
    transform.rotation = Quaternion.Lerp(transform.rotation, TargetRotation, RotatingSpeed*Time.deltaTime);
  }

  void CheckGroundContact()
  {
    Ray ray = new Ray(transform.position, Vector3.down);
    RaycastHit hit;


    if(Physics.Raycast(ray, out hit, 1.1f*height))
    {
      is_jumping = false;
    }
    else
    {
      is_jumping = true;
    }
  }

  void ApplyAdditionalGravity()
  {
    if(is_jumping)
    {
      // 1. Lerp current gravity
      current_additional_gravity = Mathf.Lerp(current_additional_gravity, AdditionalGravity, LerpGravitySpeed*Time.deltaTime);
      // 2. Apply
      rb.velocity += Vector3.down*current_additional_gravity;
    }
  }

  void AdjustDrag()
  {
    float target_drag_value =0f;
    if(is_jumping)
    {
      // target_drag_value = LerpDragSpeed.min_value;
      target_drag_value = DragStates.min_value;
      rb.drag = target_drag_value;
    }
    else
    {
      target_drag_value = is_moving ? DragStates.min_value : DragStates.max_value;
      rb.drag = target_drag_value;
      // target_drag_value = is_moving ? LerpDragSpeed.max_value : LerpDragSpeed.min_value;
      // rb.drag = LerpDragSpeed.is_immediate ? target_drag_value : Mathf.Lerp(rb.drag, target_drag_value, LerpDragSpeed.step_value*Time.deltaTime);
    }
  }

  void AdjustAnim()
  {
    // Lerps the Speed parameter
    float current_speed = Mathf.Lerp(anim.GetFloat("Speed"), Vector3.ProjectOnPlane(rb.velocity, Vector3.up).magnitude/3f,LerpAnimSpeed);
    anim.SetFloat("Speed", current_speed);

    // Lerps tilting while running parameter
    float target_angle_normalized = Mathf.Abs(TargetAngle) > 5f ? Mathf.Sign(TargetAngle) : 0f;
    target_angle_normalized = Mathf.Lerp(anim.GetFloat("RunDirection"), target_angle_normalized, LerpAnimSpeed*Time.deltaTime);
    anim.SetFloat("RunDirection", target_angle_normalized);


    //Sets X,Y parameters
    if (has_input)
    {
      float angle = TargetAngle;
      float to_rad = Mathf.Deg2Rad*angle;
      float x = Mathf.Sin(to_rad);
      float y = Mathf.Cos(to_rad);
      anim.SetFloat("X", x);
      anim.SetFloat("Y", y);
    } 
    else
    {
      anim.SetFloat("X", 0f); 
      anim.SetFloat("Y", 0f); 
    }
   

    

  }

  public void JumpControl()
  {
    Ray ray = new Ray(transform.position, - transform.up);
    RaycastHit hit;

    float current_height = MaxHeight;
    bool touched = false;

    if(Physics.Raycast(ray, out hit, MaxHeight))
    {
      current_height = hit.distance;
      touched = true;
    }

    float a = 1f/(1.1f*height - MaxHeight);
    float b = MaxHeight/(MaxHeight - 1.1f*height);
    float normalized_clip_value = Mathf.Clamp01((current_height*a + b));

    jump_lerp_current_value = Mathf.Lerp(jump_lerp_current_value, normalized_clip_value, JumpControlLerpSpeed*Time.deltaTime);

    if(normalized_clip_value > 0.85f && rb.velocity.y <0.1f)
    {
      anim.SetTrigger("Land");
      // SetAdditionalGravity(false);
      jump_lerp_current_value = InitialJumpLerpValue;
      current_additional_gravity = 0f;
      TransferSpeedOnLanding();
    }
    else
    {
      anim.Play(JumpControlAnimationName, 0,  jump_lerp_current_value);
    }
  }

  void TransferSpeedOnLanding()
  {
    Vector3 horizontal_vel = Vector3.ProjectOnPlane(rb.velocity, Vector3.up);
    if(horizontal_vel.magnitude > 1f)
    {
      rb.velocity += LandingAccelerationRatio*horizontal_vel;
    }
  }

  public void PlayerMove(Vector2 direction, bool dash, bool high_speed)
  {

    CurrentDirection = direction;
    TargetAngle = 0f;
    has_input = direction.magnitude >= 0.15f;
    CurrentDirectionCam = transform.forward;

    if(has_input)
    {
        CurrentDirectionCam = CamToPlayer(direction);
        if(CurrentBaseState == BaseStates.move)
        {
          Move(CurrentDirectionCam.normalized*Speed);
          TargetAngle = RotateTowardDirection(CurrentDirectionCam);
          is_moving = true; 
        }
        else if(CurrentBaseState == BaseStates.dash) 
        {
          Move(CurrentDirectionCam.normalized*Speed);
          TargetAngle = ComputeTargetAngle(CurrentDirectionCam); 
          is_moving = true; 
        }
        else if(CurrentBaseState == BaseStates.high_speed)
        {
          Move(CurrentDirectionCam.normalized*Speed);
          TargetAngle = ComputeTargetAngle(CurrentDirectionCam); 
          is_moving = true; 
        }
        else if(CurrentBaseState == BaseStates.prepare_throw)
        {
          Move(CurrentDirectionCam.normalized*Speed);
          TargetAngle = ComputeTargetAngle(CurrentDirectionCam); 
        }
        else
        {
          is_moving = false; 
        }

    }
    else
    {
      is_moving = false;
    }


    ManageHighSpeed(high_speed);  
    ManageDash(dash);

    CheckMaxSpeed();
    // if(jump)
    // {
    //   Jump();
    // }

  }

  public void MoveState(bool state)
  {
    if(state)
      CurrentBaseState = BaseStates.move; 
  }
  public void DashState(bool state)
  {
    if(state)
      CurrentBaseState = BaseStates.dash; 
  }
  public void HighSpeedState(bool state)
  {
    if(state)
      CurrentBaseState = BaseStates.high_speed; 
  }
  public void OtherState(bool state)
  {
    if(state)
      CurrentBaseState = BaseStates.other; 
  }
  public void PrepareThrowState(bool state)
  {
    if(state)
      CurrentBaseState = BaseStates.prepare_throw; 
  }


  public void SwitchDash(bool state)
  {
    if(CurrentBaseState == BaseStates.dash)
    {
      rb.velocity += CurrentDirectionCam*DashForce;
      SetDashTrails(true);
      GameObject explo = Instantiate(DashExplosionEffect, DashExplosionPosition.position, DashExplosionEffect.transform.rotation) as GameObject;
      Destroy(explo, 2f);
    }
    else
    {
      SetDashTrails(false);
    }
  }

  void SetDashTrails(bool enabled)
  {
    
    for(int i = 0; i<DashTrails.Length; i++)
    {
     
      DashTrails[i].enabled = enabled;
      if(enabled)
        DashTrails[i].Clear(); 
    }
  }

  void ManageDash(bool dash)
  {
    if(dash)
    {
      Dash();
    }
  }

  void ManageHighSpeed(bool high_speed)
  {
    anim.SetBool("HighSpeed", high_speed); 
  }

  void Dash()
  {
    anim.SetTrigger("Dash");
  }

  void Jump()
  {
    anim.SetTrigger("Jump");
    is_jumping = true;

    rb.velocity += Vector3.up*JumpForce;
  }

  void Move(Vector3 direction)
  {
    rb.AddForce(direction);
  }

  void Rotate(Vector2 direction)
  {
    // Vector3 look_at_direction = CamToPlayer(direction);
    Vector3 look_at_direction = CurrentDirectionCam;
    TargetRotation = Quaternion.LookRotation(look_at_direction, Vector3.up);
  }

  float RotateTowardDirection(Vector3 direction) // Lerps current rotation to match transform forward and current force applied
  {

    Quaternion rot = Quaternion.FromToRotation(transform.forward, direction);
    TargetRotation = transform.rotation*rot;

    // Debug.Log(angle);
    return Vector3.SignedAngle(transform.forward, rot*transform.forward, Vector3.up);
  }

  float ComputeTargetAngle(Vector3 direction)
  {
    Quaternion rot = Quaternion.FromToRotation(transform.forward, direction);
    return Vector3.SignedAngle(transform.forward, rot*transform.forward, Vector3.up); 
  }

  void CheckMaxSpeed()
  {
    Vector3 horizontal_vel = Vector3.ProjectOnPlane(rb.velocity, Vector3.up);
    if(horizontal_vel.magnitude > MaxSpeed)
      horizontal_vel = Vector3.Lerp(horizontal_vel, horizontal_vel.normalized*MaxSpeed, MaxSpeedLerp*Time.deltaTime);

    horizontal_vel.y = rb.velocity.y;
    rb.velocity = horizontal_vel;

  }

  Vector3 CamToPlayer(Vector2 direction)
  {
    Vector3 cam_to_player = Vector3.ProjectOnPlane(transform.position - cam.transform.position ,Vector3.up).normalized;
    Vector3 rotated_cam_to_player = Quaternion.AngleAxis(90, Vector3.up)*cam_to_player;

    return cam_to_player*direction.y + rotated_cam_to_player*direction.x;
  }

  void Initialization()
  {
    cam = Camera.main;
    rb = GetComponent<Rigidbody>();
    anim = GetComponent<Animator>();
    fight = GetComponent<BaseFight>();
    TargetRotation = transform.rotation;

    height = GetComponent<Collider>().bounds.size.y/2f;
    foreach(TrailRenderer tr in DashTrails)
    {
      tr.enabled = false;
    }
}
}


[System.Serializable]
public struct BaseLerpVector
{
  public float min_value;
  public float max_value;
  public float step_value;
  public bool is_immediate;
}

[System.Serializable]
public struct BaseImmediateChange
{
  public  float min_value;
  public  float max_value;
}
