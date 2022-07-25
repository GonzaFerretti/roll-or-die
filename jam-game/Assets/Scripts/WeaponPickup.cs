using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField]
    private WeaponDefinition weaponDef;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (weaponDef)
        {
            Initialize(weaponDef);
        }
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        Loadout loadout = col.GetComponent<Loadout>();
        if (loadout && loadout.canPickup)
        {
            loadout.AddWeapon(weaponDef, DamageSource.player);
            Destroy(gameObject);
        }
    }

    public void Initialize(WeaponDefinition inDef)
    {
        weaponDef = inDef;

        spriteRenderer.sprite = inDef.pickupIcon;
    }
}
