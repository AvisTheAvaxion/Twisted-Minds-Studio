using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIDisplayContainer : MonoBehaviour
{
    [Header("Inventory UI")]
    [SerializeField] GameObject inventoryUI;
    [SerializeField] ItemToolTip itemToolTip;

    [Header("Game UI")]
    [SerializeField] Image weaponHotbarImage;
    [SerializeField] Image weaponAbilityHotbarImage;
    [SerializeField] Image freeSlotHotbarImage;
    [SerializeField] Image mementoHotbarImage;
    [SerializeField] TextMeshProUGUI floorNumText;
    [SerializeField] TextMeshProUGUI roomNameText;
    [SerializeField] TextMeshProUGUI emotionalEnergyText;
    [SerializeField] Transform effectsIconHolder;
    [SerializeField] HeartsController heartsController;

    //[Header("Dialogue UI")]
    //[SerializeField] GameObject dialogueUIParent;
    //[SerializeField] TextMeshProUGUI dialogueNameText;
    //[SerializeField] TextMeshProUGUI dialogueText;
    //[SerializeField] Transform dialogueChoiceHolder;

    public GameObject InventoryUI { get => inventoryUI; }
    public ItemToolTip ItemToolTip { get => itemToolTip; }
    public Image WeaponHotbarImage { get => weaponHotbarImage; }
    public Image WeaponAbilityHotbarImage { get => weaponAbilityHotbarImage; }
    public Image FreeSlotHotbarImage { get => freeSlotHotbarImage; }
    public Image MementoHotbarImage { get => mementoHotbarImage; }
    public TextMeshProUGUI FloorNumText { get => floorNumText; }
    public TextMeshProUGUI RoomNameText { get => roomNameText; }
    public TextMeshProUGUI EmotionalEnergyText { get => emotionalEnergyText; }
    public Transform EffectsIconHolder { get => effectsIconHolder; }
    public HeartsController HeartsController { get => heartsController; }
}
