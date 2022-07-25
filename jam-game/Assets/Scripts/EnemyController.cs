using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private CharMovement charMovement;
    private PlayerController player;
    private Loadout loadout;

    private Manager gameManager;

    private Weapon weapon;

    [SerializeField] 
    private WeaponDefinition weaponDef;
    
    void Start()
    {
        charMovement = GetComponent<CharMovement>();
        loadout = GetComponent<Loadout>();
        loadout.SetSpawnParent(gameObject);
        weapon = loadout.AddWeapon(weaponDef, DamageSource.enemy);
        loadout.ConsumeJustPickedUpFlag();
        
        // The enemy will fire all the time
        weapon.SetIsTriggered(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (player)
        {
            Vector3 playerDirection = player.transform.position - transform.position;
            charMovement.movementInput = playerDirection;
            float angle = Mathf.Atan2(playerDirection.y, playerDirection.x);
            weapon.SetWeaponRotation(angle * Mathf.Rad2Deg);
        }
        else
        {
            player = FindObjectOfType<PlayerController>();
            charMovement.movementInput = Vector2.zero;
        }

        if (!gameManager){
            gameManager = GameObject.Find("Game Manager").GetComponent<Manager>();
        }
    }

    void OnDestroy()
    {
        gameManager.DecreaseSpawnCount("ENEMY");
    }
}
