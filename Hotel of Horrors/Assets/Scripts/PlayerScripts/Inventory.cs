using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] Weapon currentWeapon;
    [SerializeField] Item currentItem;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("TouchyTouchy");
        if(collision.TryGetComponent<Weapon>(out Weapon weapon))
        {
            currentWeapon = weapon;
            Debug.Log(weapon.GetDescription());
            collision.gameObject.SetActive(false);
        }
        else if(collision.TryGetComponent<Item>(out Item item))
        {
            currentItem = item;
            Debug.Log(item.GetDescription());
            collision.gameObject.SetActive(false);
        }
    }
}
