using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour
{
    public float checkRate = 0.05f;
    private float lastCheckTime;
    public float maxCheckDistance;
    public LayerMask layerMask;

    public GameObject curInteractGameObject;
    private IInteractable ICurInteractable;
    private ObjectType objectType;

    public TextMeshProUGUI promptText;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;

            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {
                if (hit.collider.gameObject != curInteractGameObject)
                {
                    curInteractGameObject = hit.collider.gameObject;
                    ICurInteractable = hit.collider.GetComponent<IInteractable>();
                    //objectType = hit.collider.GetComponent<EnvironmentObject>().objectData.EObjectType;
                    SetPromptText();
                }
            }
            else
            {
                curInteractGameObject = null;
                ICurInteractable = null;
                promptText.gameObject.SetActive(false);
            }
        }
    }

    private void SetPromptText()
    {
        promptText.gameObject.SetActive (true);
        promptText.text = ICurInteractable.GetInteractPrompt();
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started && ICurInteractable != null) //&& objectType == ObjectType.Info
        {
            ICurInteractable.OnInteract();
            curInteractGameObject = null;
            ICurInteractable = null;
            promptText.gameObject.SetActive(false);
        }
    }
}
