using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// 카메라 모드를 정의하는 열거형 (1인칭 / 3인칭)
public enum CameraMode
{
    FirstPerson,
    ThirdPerson
}
public class CameraMovement : MonoBehaviour
{
    [Header("Camera")]
    public float lookSensitivity = 100f; // 마우스 감도
    public float clampAngle = 70f; // 위, 아래 카메라 이동 제한

    private float rotX; 
    private float rotY;

    public Transform realCamera; // 카메라(자식)
    public Transform Player; // 캐릭터

    private Vector3 dirNormalized; // 초기 카메라 방향
    public Vector3 finalDir; // 최종 카메라 방향

    public float minDistance; // 카메라 최소 거리
    public float maxDistance; // 카메라 최대 거리
    private float finalDistance; // 최종 거리

    public float smoothness = 10f; // 카메라 이동 부드러움(Lerp)

    private Vector2 mouseDelta; // 마우스 인풋

    public CameraMode ECurrentCameraMode = CameraMode.FirstPerson;

    private void Start()
    {
        dirNormalized = realCamera.localPosition.normalized;
    }

    private void Update()
    {
        if(ECurrentCameraMode == CameraMode.FirstPerson)
        {
            rotX += mouseDelta.y * lookSensitivity * Time.deltaTime;
            rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
            transform.localEulerAngles = new Vector3(-rotX, 0, 0);

            rotY += mouseDelta.x * lookSensitivity * Time.deltaTime;
            Player.eulerAngles = new Vector3(0, rotY, 0);


        }
        else
        {
            rotX += mouseDelta.y * lookSensitivity * Time.deltaTime;
            rotY += mouseDelta.x * lookSensitivity * Time.deltaTime;

            rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
            Quaternion rot = Quaternion.Euler(rotX, rotY, 0);
            transform.rotation = rot;
        }
    }

    private void LateUpdate()
    {
        if (ECurrentCameraMode == CameraMode.FirstPerson)
        {
            realCamera.transform.localPosition = Vector3.zero;
        }
        else
        {
            finalDir = transform.TransformPoint(dirNormalized * maxDistance);

            RaycastHit hit;

            if (Physics.Linecast(transform.position, finalDir, out hit))
            {
                finalDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
            }
            else
            {
                finalDistance = maxDistance;
            }

            realCamera.localPosition = Vector3.Lerp(realCamera.localPosition, dirNormalized * finalDistance, Time.deltaTime * smoothness);
        }

    }

    public void OnLookInput(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnSwitchCameraMode(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            ECurrentCameraMode = ECurrentCameraMode == CameraMode.FirstPerson ? CameraMode.ThirdPerson : CameraMode.FirstPerson;
        }
    }
}
