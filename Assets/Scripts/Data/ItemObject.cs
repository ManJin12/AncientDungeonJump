using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData ItemData;

    public string GetInteractPrompt()
    {
        string str = $"{ItemData.objectName} \n {ItemData.objectDescription}";
        return str;
    }

    public void OnInteract()
    {
        CharacterManager.Instance.Player.itemData = ItemData;
        CharacterManager.Instance.Player.addItem?.Invoke();
        Destroy(gameObject);
    }
}
