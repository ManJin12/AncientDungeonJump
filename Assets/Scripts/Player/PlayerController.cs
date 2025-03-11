using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("MoveMent")]
    public float moveSpeed;
    public float dashPower;
    public float stepHeight;
    private Vector2 curMovementInput;
    public float jumpPower;
    public LayerMask groundLayerMask;
    public bool isDash = false;
    public bool isMoving = false;

    [Header("Look")]
    private Vector3 lookDir = Vector3.zero;
    public float lookSpeed = 3f;
    private CameraMovement cameraMovement;
    private bool toggleCameraRotation = false;

    [Header("Jump")]
    public bool isJumping = false; // 점프 중인지 체크
    public bool doublejump = false; 
    private float lastGroundTime;
    private float jumpDelay = 0.1f;

    [Header("Wall")]
    public float wallCheckDistance = 1.0f;
    public LayerMask wallLayer;
    public float grabHeightOffset = 1.5f;
    public float wallOffsetDistance = 0.5f;
    public bool wallCheck = false;

    public float climbSpeed = 2.0f;
    public bool isHanging = false;
    public bool isClimbing = false;

    private Rigidbody rigid;
    private Animator anim;
    public UIInventory inventory;
    private PlayerCondition condition;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        condition = GetComponent<PlayerCondition>();
        cameraMovement = GetComponentInChildren<CameraMovement>();
        cameraMovement.playerController = this;
    }

    private void FixedUpdate()
    {
        if (isHanging)
        {
            HandleClimbing();
            return;
        }

        Move();
    }


    private void Update()
    {
        DebugGroundCheck();
        CheckJumpState();
        MoveAnimController();
        TryGrabWall();
    }
    private void LateUpdate()
    {
        if (cameraMovement.ECurrentCameraMode == CameraMode.ThirdPerson && toggleCameraRotation == false && !isHanging)
        {
            Vector3 playerRotate = Vector3.Scale(cameraMovement.realCamera.transform.forward, new Vector3(1, 0, 1));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * cameraMovement.smoothness);
        }     
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
            isMoving = true;
            
        }
        else if(context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
            lookDir = Vector3.zero;
            isMoving = false;
        }
    }

    public void OnDashInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            isDash = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            isDash = false;
        }
    }

    private void MoveAnimController()
    {
        if (isMoving && isDash && condition.Run)
        {
            anim.SetBool("IsRun", true);
            anim.SetBool("IsWalk", false);
        }
        else if(isMoving && isDash && !condition.Run)
        {
            anim.SetBool("IsWalk", true);
            anim.SetBool("IsRun", false);
        }
        else if(isMoving && !isDash && !condition.Run)
        {
            anim.SetBool("IsWalk", true);
            anim.SetBool("IsRun", false);
        }
        else
        {
            anim.SetBool("IsRun", false);
            anim.SetBool("IsWalk", false);
        }
    }


    private void Move()
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        float speed = condition.Run ? moveSpeed * dashPower : moveSpeed;
        dir *= speed;
        dir.y = rigid.velocity.y;

        rigid.velocity = dir;

        // TopView일때 움직임 
        // 카메라 위치에 따라 캐릭터의 앞 방향이 달라져야하는데 현재 게임과는 맞지 않아 다른 방식 사용

        //lookDir.x = curMovementInput.x;
        //lookDir.z = curMovementInput.y;
        //lookDir.Normalize();

        //if (lookDir != Vector3.zero)
        //{
        //    if (Mathf.Sign(transform.forward.x) != Mathf.Sign(lookDir.x) || Mathf.Sign(transform.forward.z) != Mathf.Sign(lookDir.z))
        //    {
        //        transform.Rotate(0, 1, 0);
        //    }

        //    transform.forward = Vector3.Lerp(transform.forward, lookDir, lookSpeed * Time.deltaTime);
        //}

        //rigid.MovePosition(transform.position + lookDir * speed * Time.deltaTime);
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            if (IsGrounded())
            {
                rigid.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
                anim.SetTrigger("IsJump");
                isJumping = true;
                lastGroundTime = Time.time;
            }
            else if(!IsGrounded() && isJumping && condition.isDoubleJumping && !doublejump)
            {
                rigid.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
                anim.SetTrigger("IsJump");
                isJumping = true;
                doublejump = true;
                lastGroundTime = Time.time;
            }
             



        }

        if(context.phase == InputActionPhase.Started && isHanging)
        {
            isHanging = false;
            rigid.AddForce((Vector3.up) * jumpPower, ForceMode.Impulse);
            Debug.Log(1);
        }
    }
    
    private bool IsGrounded()
    {
        if (Time.time - lastGroundTime < jumpDelay) return false; // 점프 후 일정 시간 동안 착지 판정 X

        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down)
        };

        float rayLength = 0.3f;

        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], rayLength, groundLayerMask))
            {
                lastGroundTime = Time.time; // 땅에 닿은 시간 업데이트
                doublejump = false;
                return true;
            }
        }

        return false;
    }

    void DebugGroundCheck()
    {
        // Ray를 씬 뷰에서 보이게 하기 위한 길이
        float rayLength = 0.3f; // 기존보다 길이 늘림
        Color rayColor = Color.green; // 보기 편하게 초록색 지정

        // 4방향으로 Ray 설정
        Vector3[] rayOrigins = new Vector3[4]
        {
        transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f),
        transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f),
        transform.position + (transform.right * 0.2f) + (transform.up * 0.01f),
        transform.position + (-transform.right * 0.2f) + (transform.up * 0.01f)
        };

        // Ray를 4방향으로 그리기
        for (int i = 0; i < rayOrigins.Length; i++)
        {
            Debug.DrawRay(rayOrigins[i], Vector3.down * rayLength, rayColor, 0.1f);
        }
    }

    private void CheckJumpState()
    {
        if (isJumping)
        {
            if (rigid.velocity.y > 0.1f) // 상승 중일 때 Jump_loop 실행
            {
                anim.SetBool("IsJumping", true);
            }
            else if (rigid.velocity.y <= 0f && Time.time - lastGroundTime >= jumpDelay) // 착지 감지
            {
                anim.SetBool("IsJumping", false);
                isJumping = false; // 점프 상태 해제
            }
        }
    }

    private void TryGrabWall()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * grabHeightOffset;
        Vector3 dir = transform.forward;

        if(Physics.Raycast(origin, dir, out hit, wallCheckDistance, wallLayer))
        {
            isHanging = true;
            rigid.useGravity = false;
            rigid.velocity = Vector3.zero;

            Vector3 offset = hit.normal * wallOffsetDistance;
            transform.rotation = Quaternion.LookRotation(-offset);


            Debug.Log("벽에 매달림!");
        }
        else
        {
            isHanging = false;
            rigid.useGravity = true;
        }
    }
    private void HandleClimbing()
    {
        //transform.position += (curMovementInput.y * transform.up * climbSpeed * Time.deltaTime) 
        //    + (curMovementInput.x * transform.right * climbSpeed * Time.deltaTime);

        //if(isMoving)
        //{
        //    rigid.AddForce(curMovementInput * climbSpeed * Time.deltaTime, ForceMode.VelocityChange);
        //}
        //else
        //{
        //    rigid.velocity = Vector3.zero; // 입력이 없으면 즉시 정지
        //}

        Vector3 upwardMovement = curMovementInput.y * transform.up * climbSpeed;
        Vector3 sidewaysMovement = curMovementInput.x * transform.right * climbSpeed;
        if (isMoving)
        {
            rigid.AddForce(upwardMovement + sidewaysMovement, ForceMode.VelocityChange);
        }
        else
        {
            rigid.velocity = Vector3.zero; // 입력이 없으면 즉시 정지
        }

    }

    public void OnInventoryButton(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            string keyPressed = callbackContext.control.name; // 눌린 키의 이름 가져오기

            switch (keyPressed)
            {
                case "1":
                    inventory.UseItem(0);
                    break;
                case "2":
                    inventory.UseItem(1);
                    break;
                case "3":
                    inventory.UseItem(2);
                    break;
                case "4":
                    inventory.UseItem(3);
                    break;
                case "5":
                    inventory.UseItem(4);
                    break;
            }
        }      
    }

    public void OnRotationStop(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            toggleCameraRotation = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            toggleCameraRotation = false;
        }
    }
}
