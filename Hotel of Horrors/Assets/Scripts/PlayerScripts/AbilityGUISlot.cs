using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityGUISlot : MonoBehaviour, IPointerClickHandler
{
    AbilityGUI abilityGUI;

    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Image abilityImage;

    public int infoIndex { get; private set; }
    public int inventoryIndex { get; private set; }
    public string abilityName { get; private set; }

    private void Start()
    {
        abilityGUI = FindAnyObjectByType<AbilityGUI>();
    }

    public void SetInfoIndex(int index)
    {
        infoIndex = index;
    }
    public void SetInventoryIndex(int index)
    {
        inventoryIndex = index;
    }
    public void SetAbilityName(string name)
    {
        abilityName = name;
    }

    public void UpdateImage(AbilityInfo abilityInfo)
    {
        abilityImage.sprite = abilityInfo.GetSprite();
        canvasGroup.alpha = 0.3f;
    }
    public void UpdateImage(Ability ability)
    {
        abilityImage.sprite = ability.GetInfo().GetSprite();
        canvasGroup.alpha = 1f;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            abilityGUI.SetSelectedAbility(infoIndex);
        }
    }
}
