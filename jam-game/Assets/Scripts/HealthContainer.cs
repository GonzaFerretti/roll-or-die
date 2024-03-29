using System;
using System.Collections.Generic;
using UnityEngine;

public enum DamageSource
{
    player = 0,
    enemy = 1,
}

public enum ImmunitySource
{
    Roll = 0,
    IFrames = 1,
}

public class HealthContainer : MonoBehaviour
{
    [SerializeField]
    public float maxHP = 10;
    [SerializeField]
    private List<DamageSource> acceptedDamageSources;
    
    private HitAnimation hitAnim;

    [SerializeField]
    private WeaponDefinition arma1, arma2, arma3, arma4;

    [SerializeField]
    private GameObject pickupPrefab;

    [SerializeField] private Collider2D col;

    private List<ImmunitySource> currentImmunities;
    
    public float currentHP;

    public delegate void HealthChangeEvent(float newHP, float oldHP);
    
    public HealthChangeEvent OnDamaged;
    public HealthChangeEvent OnHealed;
    
    [SerializeField]
    private float weaponDropChance = 0.5f;
    private bool weaponDropped = false;

    public void AddImmunity(ImmunitySource source)
    {
        if (!currentImmunities.Contains(source))
        {
            currentImmunities.Add(source);
        }
    }

    public void RemoveImmunity(ImmunitySource source)
    {
        currentImmunities.Remove(source);
    }

    public void Start()
    {
        col = GetComponent<Collider2D>();
        hitAnim = GetComponent<HitAnimation>();
        currentHP = maxHP;
        currentImmunities = new List<ImmunitySource>();
    }

    public void Heal(float amount)
    {
        float oldHp = currentHP;
        currentHP = Mathf.Clamp(currentHP + amount, 0, maxHP);

        if (OnHealed != null)
        {
            OnHealed(currentHP, oldHp);
        }
    }

    public bool Hit(float damage, DamageSource source)
    {
        bool canReceiveDamage = acceptedDamageSources.Contains(source) && currentImmunities.Count == 0;
        if (canReceiveDamage)
        {
            float oldHP = currentHP;
            currentHP = Mathf.Clamp(currentHP - damage, 0, maxHP);

            if (source == DamageSource.enemy)
            {
                if (AudioManager.instance)
                {
                    AudioManager.instance.Play("playerHit");
                }
            }
            else
            {
                if (AudioManager.instance)
                {
                    AudioManager.instance.Play("enemyHit");
                }
            }
            
            hitAnim.Play();

            if (OnDamaged != null)
            {
                OnDamaged(currentHP, oldHP);
            }
        }

        if (currentHP == 0)
        {
            if(gameObject != null)
            if(!weaponDropped)
                {
                    weaponDropped = true;
                    float dropRoll = UnityEngine.Random.value;
                    
                    if (dropRoll < weaponDropChance)
                    {
                        int arma = UnityEngine.Random.Range(0, 4);
                        WeaponDefinition definition = arma1;
                        switch (arma)
                        {
                            case 0:
                                definition = arma1;
                                break;
                            case 1:
                                definition = arma2;
                                break;
                            case 2:
                                definition = arma3;
                                break;
                            case 3:
                                definition = arma4;
                                break;
                        }

                        GameObject dropPickup = Instantiate(pickupPrefab, transform.position, Quaternion.identity);
                        WeaponPickup newPickup = dropPickup.GetComponent<WeaponPickup>();
                        newPickup.Initialize(definition);

                        col.enabled = false;
                    }
                }

            Destroy(gameObject);
        }

        return canReceiveDamage;
    }
}
