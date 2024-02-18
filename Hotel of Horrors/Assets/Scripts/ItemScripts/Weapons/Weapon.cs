using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using Attacks;

[CreateAssetMenu(fileName = "Weapon", menuName = "Useables/Weapon", order = 0)]
public class Weapon : Useables
{
    
    [SerializeField] AttackModes type;
    [SerializeField] AnimationClip attackAnim;
    [SerializeField] Sprite weaponSprite;
    [SerializeField] int damage;
    [SerializeField] int attackSpeed;

    public override void Use()
    {
        Debug.Log("WEAPON USED");
    }
    public void WeaponSkill()
    {
        Debug.Log("WEAPON SKILL USED");
    }

    public AttackModes GetWeaponMode()
    {
        return type;
    }

    public Sprite GetWeaponSprite()
    {
        return weaponSprite;
    }

    public AnimationClip GetWeaponAnimation()
    {
        return attackAnim;
    }
}

