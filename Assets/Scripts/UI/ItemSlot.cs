using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public ItemData item;

    public UIInventory inventory;
    public Image icon;
    public TextMeshProUGUI quatityText;
    public Image Indicator;

    public int slotIndex;
    public int quantity;

    public void Set()
    {
        icon.sprite = item.icon;
        quatityText.text = quantity > 1 ? $"X{quantity}" : string.Empty;
        Indicator.gameObject.SetActive(false);
    }

    public void Clear()
    {
        item = null;
        icon.sprite = null;
        quatityText.text = string.Empty;
        Indicator.gameObject.SetActive(true);
    }
}
