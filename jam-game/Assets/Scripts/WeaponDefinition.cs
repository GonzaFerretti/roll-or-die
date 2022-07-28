using System;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDefinition", menuName = "Definitions/Weapon", order = 0)]
public class WeaponDefinition : ScriptableObject
{
    [SerializeField] 
    public string displayName;
    [SerializeField] 
    public Sprite pickupIcon;
    [SerializeField]
    public GameObject bullet;
    [SerializeField]
    public float range = -1;
    [SerializeField] 
    public float spread = 0;
    [SerializeField] 
    public float lifeSteal = -1;
    [SerializeField] 
    public float burstRate = 0;
    [SerializeField] 
    public int burstAmount = -1;
    [SerializeField] 
    public int amountPerShot = 1;
    [SerializeField]
    public float fireForce;
    [SerializeField] 
    public float damage;
    [SerializeField]
    public float fireInterval;
    /** Using -1 means the weapon doesn't consume ammo */
    [SerializeField] 
    public float ammo = -1;
    [SerializeField]
    public GameObject weaponPrefab;
}