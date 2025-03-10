using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IInteractable
{
    public string GetInteractPrompt();
    public void OnInteract();
}
public class EnvironmentObject : MonoBehaviour, IInteractable
{
    public ObjectData objectData;
    [SerializeField]
    private Transform targetPosition;
    private Vector3 originalPosition;

    private bool canMove = false;

    public string GetInteractPrompt()
    {
        string str = $"{objectData.objectName} \n {objectData.objectDescription}";
        return str ;
    }

    public void OnInteract()
    {
        CharacterManager.Instance.Player.objectData = objectData;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if(objectData.EObjectType == ObjectType.JumpPad)
            {
                collision.rigidbody.AddForce(Vector2.up * objectData.jumpPower, ForceMode.Impulse);
            }

            if (objectData.EObjectType == ObjectType.MovingObject)
            {
                collision.transform.parent = transform;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (objectData.EObjectType == ObjectType.MovingObject)
        {
            collision.transform.parent = null;
        } 
    }
    private void Update()
    {
        if(!canMove && objectData.EObjectType == ObjectType.MovingObject)
        {
            Move();
        }
    }

    private void Move()
    {
        StartCoroutine(MovingObject());
    }

    private IEnumerator MovingObject()
    {
        originalPosition = transform.position;;

        float elapsedTime = 0f;
        float moveDuration = 5f;
        canMove = true;
        // Move to target position
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / moveDuration);
            transform.position = Vector3.Lerp(originalPosition, targetPosition.position, t);
            yield return null;
        }

        Debug.Log(1);
        yield return new WaitForSeconds(5.0f);
        Debug.Log(2);
        elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / moveDuration);
            transform.position = Vector3.Lerp(targetPosition.position, originalPosition, t);
            yield return null;
        }
        yield return new WaitForSeconds(5.0f);
        canMove = false;
    }
}
