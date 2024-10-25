using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MementoGUI : MonoBehaviour
{
    PlayerInventory inventory;
    PlayerGUI playerGUI;

    [SerializeField] GameObject mementoUI;
    [SerializeField] Transform mementoSlotContainer;
    [SerializeField] TMP_Text emptyText;
    [SerializeField] Image equippedMementoImage;

    MementoSlot[] mementoSlots;

    // Start is called before the first frame update
    void Start()
    {
        mementoSlots = mementoSlotContainer.GetComponentsInChildren<MementoSlot>();

        inventory = FindObjectOfType<PlayerInventory>();
        playerGUI = FindObjectOfType<PlayerGUI>();

        UpdateGUI();
    }

    public void ToggleMementGUI()
    {
        mementoUI.SetActive(!mementoUI.activeSelf);

        if(mementoUI.activeSelf)
            UpdateGUI();
    }
    
    public void UpdateGUI()
    {
        int numOfMementos = inventory.GetMementos().Count;
        for (int i = 0; i < mementoSlots.Length; i++)
        {
            if(i < numOfMementos)
            {
                mementoSlots[i].gameObject.SetActive(true);
                mementoSlots[i].UpdateImage(inventory.GetMemento(i), i == inventory.currentMementoIndex);
            } else
            {
                mementoSlots[i].gameObject.SetActive(false);
            }
        }

        if(inventory.CurrentMemento != null)
        {
            //equippedMementoImage.sprite = inventory.CurrentMemento.GetSprite();
            //equippedMementoImage.color = Color.white;
        } else
        {
            //equippedMementoImage.color = Color.clear;
        }
    }

    public void EquipMemento(int index)
    {
        inventory.EquipMemento(index);
        if(!mementoSlots[index].connected)
            mementoSlots[index].ConnectTail(true);
        for (int i = 0; i < mementoSlots.Length; i++)
        {
            if(mementoSlots[i].connected && i != index)
            {
                mementoSlots[index].ConnectTail(false);
            }
        }

        UpdateGUI();
        playerGUI.UpdateHotbarGUI();
    }
}
