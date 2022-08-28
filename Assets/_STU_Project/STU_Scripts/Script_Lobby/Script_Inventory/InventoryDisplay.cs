using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryDisplay : MonoBehaviour
{
    [SerializeField] private MouseItemData mouseInventoryItem;

    protected InventorySystem inventorySystem;
    protected Dictionary<InventorySlot_UI, InventorySlot> slotDictionary = new Dictionary<InventorySlot_UI, InventorySlot>();

    public InventorySystem InventorySystem
    {
        get { return inventorySystem; }
    }

    public Dictionary<InventorySlot_UI, InventorySlot> SlotDictionary
    {
        get { return slotDictionary; }
    }
    protected virtual void Start()
    {

    }

    public abstract void AssignSlot(InventorySystem invToDisplay);

    protected virtual void UpdateSlot(InventorySlot updateSlot)
    {
        foreach (var slot in SlotDictionary)
        {
            if (slot.Value == updateSlot)
            {
                slot.Key.UpdateUISlot(updateSlot);
            }
        }
    }

    public void SlotDragBegin(InventorySlot_UI clickedUISlot)
    {
        //�p�G��e��l���F��åB�ƹ��W�S�F�� (���_)
        if (clickedUISlot.AssignedInventorySlot.ItemData != null && mouseInventoryItem.AssignedInventorySlot.ItemData == null)
        {
            mouseInventoryItem.oriUISlot = clickedUISlot;
            mouseInventoryItem.UpdateMouseSlot(clickedUISlot.AssignedInventorySlot);
            clickedUISlot.ClearSlot();
            return;
        }
    }

    public void SlotDragEnd(InventorySlot_UI clickedUISlot)
    {
        if (mouseInventoryItem.AssignedInventorySlot.ItemData == null) return;

        //�p�G��l�O�Ū��åB�ƹ��W���F�� (��J)
        if (clickedUISlot.AssignedInventorySlot.ItemData == null && mouseInventoryItem.AssignedInventorySlot.ItemData != null)
        {
            clickedUISlot.AssignedInventorySlot.AssignItem(mouseInventoryItem.AssignedInventorySlot);
            clickedUISlot.UpdateUISlot();

            mouseInventoryItem.ClearSlot();
        }

        //�p�G��l�W���F��åB�ƹ��W�]���F��
        if (clickedUISlot.AssignedInventorySlot.ItemData != null && mouseInventoryItem.AssignedInventorySlot.ItemData != null)
        {
            bool isSameItem = clickedUISlot.AssignedInventorySlot.ItemData == mouseInventoryItem.AssignedInventorySlot.ItemData;

            //�p�G�F��ۦP
            if (isSameItem)
            {
                bool isOverFlow = clickedUISlot.AssignedInventorySlot.EnoughRoomLeftInStack(mouseInventoryItem.AssignedInventorySlot.StackSize, out int leftInStack);

                //�B�[�J�ɰ��|���|���X
                if (isOverFlow)
                {
                    clickedUISlot.AssignedInventorySlot.AssignItem(mouseInventoryItem.AssignedInventorySlot);
                    clickedUISlot.UpdateUISlot();

                    mouseInventoryItem.ClearSlot();
                }
                else //���|���X
                {
                    //�p�G���Ӱ��|�O����
                    if (leftInStack < 1)
                        SwapSlots(clickedUISlot);
                    else //���|���O����
                    {
                        int remainingOnMouse = mouseInventoryItem.AssignedInventorySlot.StackSize - leftInStack;
                        clickedUISlot.AssignedInventorySlot.AddToStack(leftInStack);
                        clickedUISlot.UpdateUISlot();

                        var newItem = new InventorySlot(mouseInventoryItem.AssignedInventorySlot.ItemData, remainingOnMouse);
                        mouseInventoryItem.oriUISlot.AssignedInventorySlot.AssignItem(newItem);
                        mouseInventoryItem.oriUISlot.UpdateUISlot();
                        mouseInventoryItem.ClearSlot();
                    }
                }

            }
            //�p�G�F�褣�P�����洫
            else if (!isSameItem)
            {
                SwapSlots(clickedUISlot);
            }
        }
    }

    private void SwapSlots(InventorySlot_UI clickedUISlot)
    {
        var clonedSlot = new InventorySlot(mouseInventoryItem.AssignedInventorySlot.ItemData, mouseInventoryItem.AssignedInventorySlot.StackSize);
        mouseInventoryItem.oriUISlot.AssignedInventorySlot.AssignItem(clickedUISlot.AssignedInventorySlot);
        mouseInventoryItem.oriUISlot.UpdateUISlot();
        mouseInventoryItem.ClearSlot();

        clickedUISlot.ClearSlot();
        clickedUISlot.AssignedInventorySlot.AssignItem(clonedSlot);
        clickedUISlot.UpdateUISlot();
    }
}
