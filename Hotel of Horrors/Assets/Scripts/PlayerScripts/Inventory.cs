using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Items")]
    [SerializeField] List<Useables> inventoryItems;
    [SerializeField] public Weapon currentWeapon;
    [SerializeField] public Useables itemOne;
    [SerializeField] public Useables itemTwo;

    [Space]
    [SerializeField] GameObject inventoryUI;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("TouchyTouchy");
        if (collision.GetComponent<Useables>())
        {
            Useables useable = collision.GetComponent<Useables>();
            Debug.Log(useable.GetDescription());
            collision.gameObject.SetActive(false);
            inventoryItems.Add(useable);
            if(currentWeapon == null && useable is Weapon)
            {
                currentWeapon = (Weapon)useable;
            }
        }
    }

    public void AssignAsCurrentWeapon(GameObject gameobjectWeapon)
    {
        Weapon weapon = gameobjectWeapon.GetComponent<Weapon>();
        if(weapon != null)
        {
            currentWeapon = weapon;
            //Insert code that is done on equip like moving gameobject beside the player
        }
    }

    void OnToggleInventory()
    {
        if(inventoryUI.activeSelf == true)
        {
            inventoryUI.SetActive(false);
        }
        else
        {
            inventoryUI.SetActive(true);
        }
    }
    
}
