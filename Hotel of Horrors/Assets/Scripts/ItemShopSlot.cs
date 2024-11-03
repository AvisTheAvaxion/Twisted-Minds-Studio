using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemShopSlot : MonoBehaviour
{
    [SerializeField] Image itemDisplay;
    [SerializeField] TMP_Text itemName;
    [SerializeField] TMP_Text itemCost;
    [SerializeField] TMP_Text amountLeft;
    [SerializeField] GameObject soldOutObj;
    [SerializeField] Vector2 bobAmount = new Vector2(-100, 100);
    [SerializeField] float bobSpeed = 3f;

    Vector2 goalPos;
    Vector2 topPos;
    Vector2 bottomPos;
    float currentSpeed;
    Vector2 startPos;

    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        startPos = itemDisplay.transform.localPosition;
        currentSpeed = bobSpeed;
        goalPos = topPos = startPos + Vector2.up * bobAmount.y;
        bottomPos = startPos + Vector2.up * bobAmount.x;
    }

    // Update is called once per frame
    void Update()
    {
        float t = (Mathf.Sin(timer * Mathf.Rad2Deg) + 1) / 2f;
        itemDisplay.transform.localPosition = Vector3.Lerp(bottomPos, topPos, t);

        timer += Time.deltaTime * bobSpeed * 0.01f;
        /*if((goalPos - (Vector2)itemDisplay.transform.position).sqrMagnitude > 1f)
        {
            itemDisplay.transform.position += Vector3.up * currentSpeed * Time.deltaTime * 10;
        }
        else
        {
            currentSpeed = -currentSpeed;
            if(currentSpeed > 0)
                goalPos = startPos + Vector2.up * bobAmount.y;
            else
                goalPos = startPos + Vector2.up * bobAmount.x;
        }*/
    }

    public void SetShopItem(ShopItem shopItem)
    {
        itemDisplay.sprite = shopItem.itemSold.GetDisplaySprite();
        itemName.text = shopItem.itemSold.GetName();
        itemCost.text = shopItem.cost.ToString();
        amountLeft.text = "x" + shopItem.quantityAvailable.ToString();

        if(shopItem.quantityAvailable <= 0)
        {
            soldOutObj.SetActive(true);
            amountLeft.text = "";
        }
        else
        {
            soldOutObj.SetActive(false);
        }
    }
}
