using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum LoadoutFullResponse
{
    DiscardNewItem,
    DropCurrentWeapon,
    DropRandomWeapon,
}

public class Loadout : MonoBehaviour
{
    public delegate void OnWeaponsUpdate(Weapon[] weapons);
    public delegate void OnWeaponPickedEvent(int index);
    
    public delegate void OnCurrentWeaponChangeEvent(int index);

    public OnWeaponsUpdate OnWeaponsUpdated;
    public OnWeaponPickedEvent OnWeaponPicked;
    public OnCurrentWeaponChangeEvent OnCurrentWeaponChanged;
    
    [SerializeField] 
    private LoadoutFullResponse loadoutFullResponse;
    [SerializeField] 
    private GameObject dropPrefab;

    private Weapon[] Weapons = new Weapon[6];
    private GameObject spawnParent;

    private int currentWeaponIndex = -1;

    public bool canPickup = false;

    public bool justPickedUp = false;

    public void SetSpawnParent(GameObject inSpawnParent)
    {
        spawnParent = inSpawnParent;
    }

    public bool ConsumeJustPickedUpFlag()
    {
        bool bWasTrue = justPickedUp;
        justPickedUp = false;

        return bWasTrue;
    }

    public bool IsEmpty()
    {
        foreach (Weapon weapon in Weapons)
        {
            if (weapon != null)
            {
                return false;
            }
        }

        return true;
    }

    private int GetAvailableSlotIndex()
    {
        for (int i = 0; i < Weapons.Length; i++)
        {
            Weapon possibleSlot = Weapons[i];
            if (possibleSlot == null)
            {
                return i;
            }
        }

        return -1;
    }

    public Weapon AddWeapon(WeaponDefinition weaponDef, DamageSource damageSource, bool equipAfterwards = true)
    {
        int newWeaponIndex = GetAvailableSlotIndex();

        if (newWeaponIndex == -1)
        {
            switch (loadoutFullResponse)
            {
                case LoadoutFullResponse.DiscardNewItem:
                    return null;
                case LoadoutFullResponse.DropCurrentWeapon:
                    newWeaponIndex = currentWeaponIndex;
                    DropWeapon(currentWeaponIndex);
                    break;
                case LoadoutFullResponse.DropRandomWeapon:
                    newWeaponIndex = GetRandomAvailableWeaponIndex();
                    DropWeapon(newWeaponIndex);
                    break;
            }
        }

        if (damageSource == DamageSource.player)
        {
            if (AudioManager.instance)
            {
                AudioManager.instance.Play("pickup");
            }
        }

        GameObject newWeapon = Instantiate(weaponDef.weaponPrefab, spawnParent.transform);
        Weapon weapon = newWeapon.GetComponent<Weapon>();

        Weapons[newWeaponIndex] = weapon;
        
        weapon.Initialize(weaponDef, damageSource);

        if (OnWeaponPicked != null)
        {
            OnWeaponPicked(newWeaponIndex);
        }

        if (OnWeaponsUpdated != null)
        {
            OnWeaponsUpdated(Weapons);
        }
        
        weapon.OnAmmoDepleted += OnWeaponAmmoDepleted;
        justPickedUp = true;

        return weapon;
    }

    public void DropWeapon(int index)
    {
        Weapon weapon = Weapons[index];
        WeaponDefinition weaponDef = Instantiate(weapon.def);
        weaponDef.ammo = weapon.currentAmmo;

        GameObject dropPickup = Instantiate(dropPrefab, transform.position, Quaternion.identity);
        WeaponPickup newPickup = dropPickup.GetComponent<WeaponPickup>();
        newPickup.Initialize(weaponDef);

        Destroy(weapon.gameObject);
    }

    private void OnWeaponAmmoDepleted(Weapon weapon)
    {
        RemoveWeapon(weapon);
    }

    public void RemoveWeapon(Weapon weapon)
    {
        for (int i = 0; i < Weapons.Length; i++)
        {
            Weapon weaponSlot = Weapons[i];
            if (weaponSlot == weapon)
            {
                Destroy(weaponSlot.gameObject);
                Weapons[i] = null;
                if (OnWeaponsUpdated != null)
                {
                    OnWeaponsUpdated(Weapons);
                }
                SetWeaponIndex(-1);
                break;
            }
        }
    }

    public void SetRandomWeapon()
    {
        int randomIndex = GetRandomAvailableWeaponIndex();

        if (randomIndex != -1)
        {
            SetWeaponIndex(randomIndex);
        }
    }

    private int GetRandomAvailableWeaponIndex()
    {
        List<int> PossibleSlots = new List<int>();
        for (int i = 0; i < Weapons.Length; i++)
        {
            Weapon weaponSlot = Weapons[i];
            if (weaponSlot != null && i != currentWeaponIndex)
            {
                PossibleSlots.Add(i);
            }
        }
        
        if (PossibleSlots.Count == 0)
        {
            return -1;
        }

        int randomIndex = Random.Range(0, PossibleSlots.Count);
        return PossibleSlots[randomIndex];
    }

    public void SetWeaponIndex(int newIndex)
    {
        if (newIndex == currentWeaponIndex)
        {
            return;
        }
        
        currentWeaponIndex = newIndex;

        for (int i = 0; i < Weapons.Length; i++)
        {
            Weapon weapon = Weapons[i];
            if (weapon)
            {
                weapon.gameObject.SetActive(i == currentWeaponIndex);
            }
        }

        if (OnCurrentWeaponChanged != null)
        {
            OnCurrentWeaponChanged(currentWeaponIndex);
        }
    }

    public Weapon GetCurrentWeapon()
    {
        return currentWeaponIndex != -1 ? Weapons[currentWeaponIndex] : null;
    }
}
