using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Bullet : MonoBehaviour
{
    public DamageSource source;

    public float damage;
    public Vector3 origin;
    public float rangeSqr = -1;

    public void Update()
    {
        if (rangeSqr != -1 && (origin - transform.position).sqrMagnitude > rangeSqr)
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
            shouldDestroy = container.Hit(damage, source);
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
