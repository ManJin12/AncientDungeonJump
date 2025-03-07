using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum ItemType
{
    Health,
    Speed,
    Stamina
}

[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string objectName;
    public string objectDescription;
    public ItemType EItemType;
    public Sprite icon; 

    [Header("Value")]
    public float value;
    public float duration;
    public int maxStackAmount;
    public bool canStack;
}
