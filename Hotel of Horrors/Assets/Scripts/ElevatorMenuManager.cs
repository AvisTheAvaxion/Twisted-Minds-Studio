using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ElevatorMenuManager : MonoBehaviour
{
    [Header ("Menu References and options")]
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject loadMenu;
    [SerializeField] GameObject storeMenu;
    [SerializeField] GameObject savedText;
    [SerializeField] GameObject loadText;
    [SerializeField] GameObject elevatorBackground;
    [SerializeField] GameObject mindRoomMenu;
    [SerializeField] Image fadeImage;
    [SerializeField] float doorTransitionLength = 0.5f;
    [Header ("Class References")]
    [SerializeField] PlayerMovement player;
    [SerializeField] NewAudioManager audioManager;
    [SerializeField] SerializationManager serializationManager;
    [Header("Shop Items")]
    [SerializeField] protected ShopItem[] shopItems;
    [SerializeField] Button[] itemButtons;

    private void Start()
    {
        elevatorBackground.SetActive(false);

        
    }

    #region navigation
    public void SwitchToMain()
    {
        mainMenu.SetActive(true);
        loadMenu.SetActive(false);
        storeMenu.SetActive(false);
    }

    public void SwitchToStore()
    {
        mainMenu.SetActive(false);
        loadMenu.SetActive(false);
        storeMenu.SetActive(true);

        for (int i = 0; i < itemButtons.Length; i++)
        {
            if (i >= shopItems.Length)
            {
                itemButtons[i].gameObject.SetActive(false);
                continue;
            }


            itemButtons[i].GetComponent<Image>().sprite = shopItems[i].itemSold.GetSprite();
            itemButtons[i].GetComponentInChildren<TMP_Text>().text = shopItems[i].itemSold.GetName() + " - " + shopItems[i].cost + "EE (" + shopItems[i].quantityAvailable + " left)";
        }
    }

    public void SwitchToLoad()
    {
        loadText.SetActive(true);
        serializationManager.LoadData();
        //mainMenu.SetActive(false);
        //loadMenu.SetActive(true);
        //storeMenu.SetActive(false);
    }

    public void SaveGame()
    {
        savedText.SetActive(true);

        //save game here
        serializationManager.SaveData();
    }

    public void ReturnToPrevRoom()
    {
        StartCoroutine(GetFreshRoom());
    }

    public void LeaveMindMenu()
    {
        Cursor.visible = false;
        mindRoomMenu.SetActive(false);
    }

    IEnumerator GetFreshRoom()
    {
        print("fading black");
        //audioManager.PlayEffect("DoorOpen");
       
        // loop over 1 second - fade to black
        for (float i = 0; i <= doorTransitionLength / 2f; i += Time.deltaTime)
        {
            // set color with i as alpha
            fadeImage.color = new Color(0, 0, 0, i / (doorTransitionLength / 2f));
            yield return null;
        }


        print("fading clear");
        // loop over 1 second backwards - fade to clear
        for (float i = doorTransitionLength / 2f; i >= 0; i -= Time.deltaTime)
        {
            // set color with i as alpha
            fadeImage.color = new Color(0, 0, 0, i / (doorTransitionLength / 2f));
            yield return null;
        }
        player.canMove = true;
        elevatorBackground.SetActive(false);
    }
    #endregion

    #region shop logic

    public void BuyItem(int itemIndex)
    {
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();

        if (shopItems[itemIndex].quantityAvailable >= 1 && inventory.emotionalEnergy >= shopItems[itemIndex].cost)
        {
            

            inventory.AddItem(new Item(shopItems[itemIndex].itemSold, 1));
            inventory.SubtractEmotionialEnergy(shopItems[itemIndex].cost);
            shopItems[itemIndex].quantityAvailable--;

            itemButtons[itemIndex].GetComponentInChildren<TMP_Text>().text = shopItems[itemIndex].itemSold.GetName() + " - " + shopItems[itemIndex].cost + "EE (" + shopItems[itemIndex].quantityAvailable + " left)";
        }

    }

    #endregion

}

[System.Serializable]
public struct ShopItem
{
    public ItemInfo itemSold;
    public int cost;
    public int quantityAvailable;
}