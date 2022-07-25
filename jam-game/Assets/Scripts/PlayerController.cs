using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private WeaponDefinition defaultWeaponDef;

    [SerializeField] 
    private float sideRollThreshold = 15;
    
    private PlayerMovement playerMovement;
    private SpriteRenderer spriteRenderer;
    private HealthContainer healthContainer;
    private PlayerLook playerLook;
    private Animator animator;
    private HudController hud;
    [HideInInspector]
    public Loadout loadout;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        healthContainer = GetComponent<HealthContainer>();
        playerMovement = GetComponent<PlayerMovement>();

        float rollMultiplier = playerMovement.rollDuration * (12f/7f);
        animator.SetFloat("rollSpeed", 1/rollMultiplier);
        playerMovement.OnRolledEvent += OnRolledEvent;
        hud = FindObjectOfType<HudController>();

        playerLook = GetComponent<PlayerLook>();
        loadout = GetComponent<Loadout>();
        
        loadout.OnWeaponPicked += OnWeaponPicked;
        loadout.OnWeaponsUpdated += hud.UpdateIcons;
        loadout.OnCurrentWeaponChanged += hud.UpdateActiveWeapon;
        
        loadout.SetSpawnParent(gameObject);
        loadout.AddWeapon(defaultWeaponDef, DamageSource.player);
    }

    private void OnRolledEvent(bool bIsStart)
    {
        if (bIsStart)
        {
            animator.SetTrigger("Roll");
            
            healthContainer.AddImmunity(ImmunitySource.Roll);
            loadout.canPickup = true;

            int direction = -1;
            
            direction = GetAnimDirectionIndex(playerMovement.rollDirection);

            animator.SetInteger("Direction", direction);

            if (AudioManager.instance)
            {
                AudioManager.instance.Play("roll");
            }
        }
        else
        {
            if (!loadout.ConsumeJustPickedUpFlag())
            {
                loadout.SetRandomWeapon();
            }
            healthContainer.RemoveImmunity(ImmunitySource.Roll);
            loadout.canPickup = false;
        }
        
        Weapon CurrentWeapon = loadout.GetCurrentWeapon();
        if (CurrentWeapon)
        {
            CurrentWeapon.gameObject.SetActive(!bIsStart);
        }
    }

    private int GetAnimDirectionIndex(Vector2 lookingDirection)
    {
        int direction = -1;
        
        float rollAngle = Mathf.Atan2(lookingDirection.y, lookingDirection.x) * Mathf.Rad2Deg;
        rollAngle = (rollAngle < 0) ? rollAngle + 360 : rollAngle;
        if (rollAngle >= 0 && rollAngle < 90 - sideRollThreshold / 2)
        {
            direction = 0;
        }
        else if (rollAngle >= 90 - sideRollThreshold / 2 && rollAngle < 90 + sideRollThreshold / 2)
        {
            direction = 1;
        }
        else if (rollAngle >= 90 + sideRollThreshold / 2 && rollAngle < 180)
        {
            direction = 0;
        }
        else if (rollAngle >= 180 && rollAngle < 270 - sideRollThreshold)
        {
            direction = 3;
        }
        else if (rollAngle >= 270 - sideRollThreshold && rollAngle < 270 + sideRollThreshold)
        {
            direction = 2;
        }
        else if (rollAngle >= 270 + sideRollThreshold && rollAngle < 360)
        {
            direction = 3;
        }

        return direction;
    }

    private void OnWeaponPicked(int index)
    {
        loadout.SetWeaponIndex(index);
        loadout.canPickup = false;
    }

    public void BounceFromBoundary()
    {
        Vector3 BounceVector = playerMovement.isRolling
            ? -playerMovement.rollDirection
            : new Vector3(-playerMovement.movementInput.x, -playerMovement.movementInput.y);

        BounceVector = BounceVector.normalized;
        transform.position += BounceVector * 1;
        playerMovement.InterruptRoll(BounceVector);
    }

    void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        if (hud && healthContainer)
        {
            hud.SetHPBar(healthContainer.currentHP, healthContainer.maxHP);
        }
        
        playerMovement.movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        animator.SetBool("IsMoving", playerMovement.movementInput.sqrMagnitude != 0);
        
        if (Input.GetKey(KeyCode.Space))
        {
            playerMovement.TryRoll();
        }

        Weapon weapon = loadout.GetCurrentWeapon();

        if (playerMovement.isRolling)
        {
            spriteRenderer.flipX = playerMovement.rollDirection.x < 0;
        }
        
        if (weapon)
        {
            weapon.SetIsTriggered(Input.GetMouseButton(0) && !playerMovement.isRolling);
            weapon.SetWeaponRotation(playerLook.GetLookAngle() * Mathf.Rad2Deg);

            Vector2 lookDirection = playerLook.GetLookDirection();

            if (!playerMovement.isRolling)
            {
                spriteRenderer.flipX = lookDirection.x < 0;
            }

            if (lookDirection != Vector2.zero)
            {
                animator.SetInteger("Direction", GetAnimDirectionIndex(lookDirection));
            }
        }
        else
        {
            if (playerMovement.movementInput.x != 0 && !playerMovement.isRolling)
            {
                spriteRenderer.flipX = playerMovement.movementInput.x < 0;
            }

            if (playerMovement.movementInput != Vector2.zero)
            {
                animator.SetInteger("Direction", GetAnimDirectionIndex(playerMovement.movementInput.normalized));
            }
        }
    }
}
