using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIDisplayContainer : MonoBehaviour
{
    [Header("Inventory UI")]
    [SerializeField] GameObject inventoryUI;
    [SerializeField] GameObject weaponsContainer;
    [SerializeField] GameObject itemsContainer;
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
    [SerializeField] GameObject effectsIconPrefab;
    [SerializeField] HeartsController heartsController;
    [SerializeField] GameObject ammoTextContainer;
    [SerializeField] TextMeshProUGUI currentAmmoText;
    [SerializeField] TextMeshProUGUI maxAmmoText;
    [SerializeField] Image reloadCircle;
    [SerializeField] Slider boss_healthBar;

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
    public GameObject AmmoTextContainer { get => ammoTextContainer; }
    public TextMeshProUGUI CurrentAmmoText { get => currentAmmoText; }
    public TextMeshProUGUI MaxAmmoText { get => maxAmmoText; }
    public Image ReloadCircle { get => reloadCircle; }
    public GameObject EffectsIconPrefab { get => effectsIconPrefab; }
    public Slider Boss_healthBar { get => boss_healthBar;}
    public GameObject WeaponsContainer { get => weaponsContainer; }
    public GameObject ItemsContainer { get => itemsContainer; }
}
