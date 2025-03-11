using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    public ItemSlot[] slots;

    public Transform slotPanel;

    [Header("Selected Item")]
    private ItemSlot seletedItem;
    private int seletedItemIndex;

    private PlayerController playerController;
    private PlayerCondition playerCondition;

    private void Start()
    {
        playerController = CharacterManager.Instance.Player.controller;
        playerCondition = CharacterManager.Instance.Player.condition;
        CharacterManager.Instance.Player.controller.inventory = this;

        CharacterManager.Instance.Player.addItem += AddItem;

        slots = new ItemSlot[slotPanel.childCount];

        for(int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>();
            slots[i].slotIndex = i;
            slots[i].inventory = this;
            slots[i].Clear();
        }
    }

    public void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                slots[i].Set();
            }
            else
            {
                slots[i].Clear();
            }
        }
    }

    public void AddItem()
    {
        ItemData itemData = CharacterManager.Instance.Player.itemData;

        if(itemData.canStack)
        {
            ItemSlot slot = GetItemStack(itemData);
            if(slot != null)
            {
                slot.quantity++;
                UpdateUI();
                CharacterManager.Instance.Player.itemData = null;
                return;
            }
        }

        ItemSlot emptySlot = GetEmptySlot();

        if (emptySlot != null)
        {
            emptySlot.item = itemData;
            emptySlot.quantity = 1;
            UpdateUI();
            CharacterManager.Instance.Player.itemData = null;
            return;
        }

        CharacterManager.Instance.Player.itemData = null;
    }

    private ItemSlot GetItemStack(ItemData data)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == data && slots[i].quantity < data.maxStackAmount)
            {
                return slots[i];
            }
        }
        return null;
    }

    private ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                return slots[i];
            }
        }
        return null;
    }

    public void UseItem(int useIndex)
    {
        if(slots[useIndex].item == null) return;

        switch (slots[useIndex].item.EItemType)
        {
            case ItemType.Health:
                playerCondition.Heal(slots[useIndex].item.value);
                ReMoveItem(useIndex);
                break;
            case ItemType.Speed:
                playerCondition.SpeedUp(slots[useIndex].item.value, slots[useIndex].item.duration);
                ReMoveItem(useIndex);
                break;
            case ItemType.Stamina:
                playerCondition.AddStamina(slots[useIndex].item.value);
                ReMoveItem(useIndex);
                break;
            case ItemType.DoubleJump:
                playerCondition.Jumping(slots[useIndex].item.duration);
                ReMoveItem(useIndex);
                break;
        }
    }

    private void ReMoveItem(int useIndex)
    {
        slots[useIndex].quantity--;

        if( slots[useIndex].quantity <= 0 )
        {
            slots[useIndex].item = null;
        }

        UpdateUI(); ;
    }
}
