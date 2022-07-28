using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public delegate void BulletHitEvent(BulletParams bulletParams, GameObject hitObject);

public class Bullet : MonoBehaviour
{
    public BulletHitEvent OnHit;
    
    public DamageSource source;
    public BulletParams bulletParams;
    public HealthContainer ownerHealthContainer;

    public Vector3 origin;

    Bullet()
    {
        bulletParams = new BulletParams();
    }

    public void Update()
    {
        if (bulletParams.sqrRange != -1 && (origin - transform.position).sqrMagnitude > bulletParams.sqrRange)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        bool shouldDestroy = false;
        HealthContainer container = col.gameObject.GetComponent<HealthContainer>();
        if (container)
        {
            bool Hit = container.Hit(bulletParams.damage, source);
            shouldDestroy = Hit;
            if (Hit && OnHit != null)
            {
                OnHit(bulletParams, col.gameObject);
            }
        }
        else
        {
            shouldDestroy = true;
        }

        if (shouldDestroy)
        {
            Destroy(gameObject);
        }
       
    }
}

public class BulletParams
{
    public float damage = 0.0f;
    public float sqrRange = -1.0f;
    public float healthOnHit = -1.0f;

    public BulletParams()
    {
        
    }

    public BulletParams(WeaponDefinition weaponDef)
    {
        damage = weaponDef.damage;
        sqrRange = (weaponDef.range == -1.0f) ? -1.0f : weaponDef.range * weaponDef.range;
        healthOnHit = weaponDef.lifeSteal;
    }
}
