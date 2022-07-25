using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    
    private Manager manager;

    [SerializeField]
    private GameObject entityToSpawn;
    [SerializeField]
    private float maxSpawnDistance;
    [SerializeField]
    private string spawnerType;

    // Update is called once per frame
    void FixedUpdate()
    {
        bool bWasNotBefore = !manager;
        if (!manager)
        {
            manager = GameObject.Find("Game Manager").GetComponent<Manager>();
        }

        if (bWasNotBefore && manager)
        {
            manager.SubscribeSpawner(spawnerType, this);
        }
    }


    public GameObject Activate()
    {
        //Select random spawn position
        float d = maxSpawnDistance/2;
        float x_mod = Random.Range(-d,d);
        float y_mod = Random.Range(-d,d);
        
        Vector3 spawnPos = transform.position + new Vector3(x_mod, y_mod);

        if (spawnerType == "WEAPON"){
            return _SpawnWeapon(spawnPos);
        } else {
            return Instantiate(entityToSpawn, spawnPos, Quaternion.identity);
        }
    }

    private GameObject _SpawnWeapon(Vector3 spawnPos)
    {   
        WeaponDefinition weaponDef = _ChooseWeapon();
        GameObject dropPickup = Instantiate(entityToSpawn, spawnPos, Quaternion.identity);
        WeaponPickup newPickup = dropPickup.GetComponent<WeaponPickup>();
        newPickup.Initialize(weaponDef);
        return dropPickup;
    }

    private WeaponDefinition _ChooseWeapon()
    {
        List<WeaponDefinition> aw = manager.AvailableWeapons;

        return aw[Random.Range(0,aw.Count)];
    }
}
