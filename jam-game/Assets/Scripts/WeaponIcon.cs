using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponIcon : MonoBehaviour
{
    private Weapon weapon;
    private Image bgImage;
    
    [SerializeField]
    private Image ammoBar;
    
    [SerializeField]
    private Image ammoBarBg;

    [SerializeField] 
    private Image iconImage;

    private Color defaultColor;

    private void Awake()
    {
        bgImage = GetComponent<Image>();
        defaultColor = bgImage.color;
    }

    public void SetWeapon(Weapon inWeapon)
    {
        if (weapon)
        {
            weapon.OnAmmoChanged -= OnAmmoChanged;
        }
        
        weapon = inWeapon;
            
        iconImage.enabled = weapon;
        ammoBarBg.enabled = weapon;
        if (weapon)
        {
            iconImage.sprite = weapon.def.pickupIcon;
            OnAmmoChanged(weapon, weapon.currentAmmo, weapon.def.ammo);
            weapon.OnAmmoChanged += OnAmmoChanged;
        }
        else
        {
            iconImage.sprite = null;
        }
    }

    public void OnAmmoChanged(Weapon weapon, float newAmount, float maxAmount)
    {
        ammoBar.fillMethod = Image.FillMethod.Horizontal;
        ammoBar.fillAmount = newAmount / maxAmount;
    }

    public void SetActiveWeapon(bool isActive)
    {
        bgImage.color = isActive ? new Color(0.36f, 0.61f, 0.2f, 0.69f) : defaultColor;
    }
}
