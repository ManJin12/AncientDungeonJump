using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// ī�޶� ��带 �����ϴ� ������ (1��Ī / 3��Ī)
public enum CameraMode
{
    FirstPerson,
    ThirdPerson
}
public class CameraMovement : MonoBehaviour
{
    [Header("Camera")]
    public float lookSensitivity = 100f; // ���콺 ����
    public float clampAngle = 70f; // ��, �Ʒ� ī�޶� �̵� ����

    private float rotX; 
    private float rotY;

    public Transform realCamera; // ī�޶�(�ڽ�)
    public Transform Player; // ĳ����

    private Vector3 dirNormalized; // �ʱ� ī�޶� ����
    public Vector3 finalDir; // ���� ī�޶� ����

    public float minDistance; // ī�޶� �ּ� �Ÿ�
    public float maxDistance; // ī�޶� �ִ� �Ÿ�
    private float finalDistance; // ���� �Ÿ�

    public float smoothness = 10f; // ī�޶� �̵� �ε巯��(Lerp)

    private Vector2 mouseDelta; // ���콺 ��ǲ

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
