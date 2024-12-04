using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ElevatorMenuManager : MonoBehaviour
{
    [Header ("Menu References and options")]
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject loadMenu;
    [SerializeField] GameObject storeMenu;
    [SerializeField] GameObject savedText;
    [SerializeField] float saveNotifyLength = 1.5f;
    [SerializeField] GameObject loadText;
    [SerializeField] GameObject elevatorBackground;
    [SerializeField] GameObject mindRoomMenu;
    [SerializeField] GameObject nextLevelButton;
    [SerializeField] Image fadeImage;
    [SerializeField] float doorTransitionLength = 0.5f;
    [Header ("Class References")]
    [SerializeField] PlayerMovement player;
    [SerializeField] NewAudioManager audioManager;
    [SerializeField] SerializationManager serializationManager;
    [Header("Shop Items")]
    [SerializeField] protected ShopItem[] shopItems;
    protected ShopItem[] itemsForSale;
    int shopSize = 4;
    List<int> badIndexes = new List<int>();
    [SerializeField] ItemShopSlot[] itemShopSlots;

    public bool elevatorMenuOpen { get; private set; }

    QuestSystem questSystem;


    private void Start()
    {
        elevatorBackground.SetActive(false);
        itemsForSale = new ShopItem[4];

        elevatorMenuOpen = false;

        questSystem = FindObjectOfType<QuestSystem>();
        if(questSystem.FloorCleared)
        {
            nextLevelButton.SetActive(true);
        }
        else
        {
            nextLevelButton.SetActive(false);
        }

        ResetShop();
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

        for (int i = 0; i < itemShopSlots.Length; i++)
        {
            if (i >= shopSize)
            {
                itemShopSlots[i].gameObject.SetActive(false);
                continue;
            }

            itemShopSlots[i].SetShopItem(itemsForSale[i]);

            //itemButtons[i].GetComponent<Image>().sprite = itemsForSale[i].itemSold.GetSprite();
            //itemButtons[i].GetComponentInChildren<TMP_Text>().text = itemsForSale[i].itemSold.GetName() + " - " + itemsForSale[i].cost + "EE (" + itemsForSale[i].quantityAvailable + " left)";
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

    public void GoToNextFloor()
    {
        serializationManager.SaveData();

        SceneManager.LoadScene("EndDemo");
    }
    public void GoToBossFloor()
    {
        serializationManager.SaveData();

        SceneManager.LoadScene("FinalBoss");
    }

    public void SaveGame()
    {
        if (saveNotifyCoroutine != null) StopCoroutine(saveNotifyCoroutine);

        saveNotifyCoroutine = StartCoroutine(SaveNotification());

        //save game here
        serializationManager.SaveData();
    }
    Coroutine saveNotifyCoroutine;
    IEnumerator SaveNotification()
    {
        savedText.SetActive(true);
        yield return new WaitForSeconds(saveNotifyLength);
        savedText.SetActive(false);
    }

    public void ReturnToPrevRoom()
    {
        StartCoroutine(GetFreshRoom());
    }

    public void OpenElevatorMenu()
    {
        elevatorMenuOpen = true;
        elevatorBackground.SetActive(true);

        audioManager.PlayEffect("ElevatorJam");
    }

    public void LeaveMindMenu()
    {
        //Cursor.visible = false;
        mindRoomMenu.SetActive(false);
    }

    IEnumerator GetFreshRoom()
    {
        elevatorMenuOpen = false;
        //audioManager.PlayEffect("DoorOpen");

        // loop over 1 second - fade to black
        for (float i = 0; i <= doorTransitionLength / 2f; i += Time.deltaTime)
        {
            // set color with i as alpha
            fadeImage.color = new Color(0, 0, 0, i / (doorTransitionLength / 2f));
            yield return null;
        }


        // loop over 1 second backwards - fade to clear
        for (float i = doorTransitionLength / 2f; i >= 0; i -= Time.deltaTime)
        {
            // set color with i as alpha
            fadeImage.color = new Color(0, 0, 0, i / (doorTransitionLength / 2f));
            yield return null;
        }
        player.SetCanMove(true);
        GameState.CurrentState = GameState.State.None;
        elevatorBackground.SetActive(false);
    }
    #endregion

    #region shop logic

    public void BuyItem(int itemIndex)
    {
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();

        if (itemsForSale[itemIndex].quantityAvailable >= 1 && inventory.emotionalEnergy >= itemsForSale[itemIndex].cost)
        {
            inventory.AddItem(new Item(itemsForSale[itemIndex].itemSold, 1));
            inventory.SubtractEmotionialEnergy(itemsForSale[itemIndex].cost);
            itemsForSale[itemIndex].quantityAvailable--;

            itemShopSlots[itemIndex].SetShopItem(itemsForSale[itemIndex]);
            //itemShopSlots[itemIndex].GetComponentInChildren<TMP_Text>().text = itemsForSale[itemIndex].itemSold.GetName() + " - " + itemsForSale[itemIndex].cost + "EE (" + itemsForSale[itemIndex].quantityAvailable + " left)";
        }

    }

    public void ResetShop()
    {
        int maxToSell = 4;
        if (shopItems.Length < maxToSell)
            maxToSell = shopItems.Length;

        Random.seed = (int)System.DateTime.Now.Millisecond;

        for (int i = 0; i < 10; i++)
        {
            shopSize = Random.Range(2, maxToSell + 1);
        }
        badIndexes.Clear();

        for (int i = 0; i < shopSize; i++)
        {
            itemsForSale[i] = shopItems[getShopIndex()];
            itemsForSale[i].quantityAvailable = itemsForSale[i].defaultQuantity;
        }
    }

    int getShopIndex()
    {
        int index = Random.Range(0, shopItems.Length);

        if (badIndexes.Contains(index))
            return getShopIndex();

        badIndexes.Add(index);
        return index;
    }

    #endregion

    public void AllowMoveToNextFloor()
    {
        nextLevelButton.SetActive(true);
    }
}

[System.Serializable]
public struct ShopItem
{
    public ItemInfo itemSold;
    public int cost;
    public int quantityAvailable;
    public int defaultQuantity;
}