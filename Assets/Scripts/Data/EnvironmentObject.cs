using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public string GetInteractPrompt();
    public void OnInteract();
}
public class EnvironmentObject : MonoBehaviour, IInteractable
{
    public ObjectData objectData;

    public string GetInteractPrompt()
    {
        string str = $"{objectData.objectName} \n {objectData.objectDescription}";
        return str ;
    }

    public void OnInteract()
    {
        CharacterManager.Instance.Player.objectData = objectData;
    }
}
