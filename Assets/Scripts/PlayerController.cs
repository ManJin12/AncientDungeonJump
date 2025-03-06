using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("MoveMent")]
    public float moveSpeed;
    public float stepHeight;
    private Vector2 curMovementInput;
    public float jumpPower;
    public LayerMask groundLayerMask;
    public LayerMask stairsLayerMask;

    [Header("Look")]
    public Transform cameraPosition;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot;
    private float camCurYRot;
    public float lookSensitivity;

    private Vector2 mouseDelta;
    private Rigidbody rigid;
    //private Animator anim;

    private bool isJumping = false; // 점프 중인지 체크


    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        //anim = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        Move();
    }


    private void Update()
    {

    }
    private void LateUpdate()
    {
        Look();
    }

    public void OnLookInput(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    private void Look()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraPosition.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        camCurYRot += mouseDelta.x * lookSensitivity;
        transform.eulerAngles = new Vector3(0, camCurYRot, 0);
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if(context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
        }
       
    }

    private void Move()
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        dir *= moveSpeed;
        dir.y = rigid.velocity.y;

        rigid.velocity = dir;
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started && IsGrounded())
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
        }
    }
    
    private bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down)
        };

        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.3f, groundLayerMask))
            {
                return true;
            }
        }

        return false;

    }
}
