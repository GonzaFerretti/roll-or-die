using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Weapon : MonoBehaviour
{
    public delegate void OnAmmoDepletion(Weapon weapon);

    public delegate void OnAmmoChangedEvent(Weapon weapon, float newAmount, float maxAmount);

    public OnAmmoDepletion OnAmmoDepleted;
    public OnAmmoChangedEvent OnAmmoChanged;
    public BulletHitEvent OnBulletHit;

    [SerializeField]
    private Transform firePoint;
    [SerializeField]
    private Transform rotationPivot;

    [SerializeField] 
    private SpriteRenderer spriteRenderer;

    public WeaponDefinition def;

    private float lastShotTimestamp = -1f;
    private int remainingBurstFires = 0;
    private bool isTriggered = false;
    public float currentAmmo;

    public DamageSource damageSource;

    public void Initialize(WeaponDefinition weaponDef, DamageSource inDamageSource)
    {
        def = weaponDef;
        currentAmmo = weaponDef.ammo;
        damageSource = inDamageSource;
    }

    public void SingleFire()
    {
        TryFire();
    }

    public void SetIsTriggered(bool newState)
    {
        if (newState != isTriggered)
        {
            isTriggered = newState;

            if (isTriggered)
            {
                TryFire();
            }
        }
    }

    public void Update()
    {
        if(Time.timeScale == 0)
        {
            return;
        }

        if (isTriggered)
        {
            TryFire();
        }
        
        bool bUsesBurst = def.burstAmount != -1;
        float currentTime = Time.time;
        if (bUsesBurst && remainingBurstFires > 0 && currentTime >= lastShotTimestamp + def.burstRate)
        {
            remainingBurstFires--;
            Fire(currentTime);
        }

        if (spriteRenderer)
        {
            float normalizedAngle = rotationPivot.eulerAngles.z;
            normalizedAngle = (normalizedAngle < 0) ? normalizedAngle + 360 : normalizedAngle;
            spriteRenderer.flipY = normalizedAngle > 0 && normalizedAngle <= 180;
        }
    }

    public void SetWeaponRotation(float angle)
    {
        rotationPivot.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    private void TryFire()
    {
        float currentTime = Time.time;
        if (def.fireInterval == -1.0f || currentTime >= lastShotTimestamp + def.fireInterval)
        {
            if (damageSource == DamageSource.player)
            {
                if (AudioManager.instance)
                {
                    AudioManager.instance.Play("weaponFire");
                }
            }
            
            if (remainingBurstFires == 0 && def.burstAmount != 1)
            {
                remainingBurstFires = def.burstAmount - 1;
            }
            Fire(currentTime);
        }
    }

    private void Fire(float currentTime)
    {
        lastShotTimestamp = currentTime;

        Quaternion firePointRotation = rotationPivot.rotation;

        for (int i = def.amountPerShot; i > 0; --i)
        {
            Quaternion localRotation = firePointRotation;
            if (def.spread != 0)
            {
                float randomAngleDelta = Random.Range(-def.spread / 2, def.spread / 2);
                localRotation *= Quaternion.Euler(randomAngleDelta * Vector3.forward);
            }

            GameObject projectile = Instantiate(def.bullet, firePoint.position, localRotation);
            projectile.transform.up = Quaternion.Euler(0,0,90) * localRotation * Vector3.up;
            projectile.GetComponent<Rigidbody2D>().AddForce((localRotation * Vector3.up) * def.fireForce, ForceMode2D.Impulse);
            Bullet createdBullet = projectile.GetComponent<Bullet>();
            createdBullet.source = damageSource;
            createdBullet.bulletParams = new BulletParams(def);
            createdBullet.origin = firePoint.position;
            createdBullet.OnHit += NotifyBulletHit;
        }
        
        if (def.ammo != -1)
        {
            currentAmmo--;
            if (OnAmmoChanged != null)
            {
                OnAmmoChanged(this, currentAmmo, def.ammo);
            }
            
            if (currentAmmo == 0)
            {
                if (OnAmmoDepleted != null)
                {
                    OnAmmoDepleted(this);
                }
            }
        }
    }

    private void NotifyBulletHit(BulletParams bulletParams, GameObject hitobject)
    {
        if (OnBulletHit != null)
        {
            OnBulletHit(bulletParams, hitobject);
        }
    }
}
