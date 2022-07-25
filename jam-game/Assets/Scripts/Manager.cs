using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Manager : MonoBehaviour
{
    private PlayerController player;
    
    private GameObject gameOverScreen;
    [SerializeField]
    private int InitialWaveEnemies = 4;
    [SerializeField]
    private int WaveEnemyIncrease = 2;
    [SerializeField]
    private int WaveCooldownSecs = 10;
    [SerializeField] 
    private int EmergencyWeaponCooldown = 15;
    private int cooldown = 0;
    private bool paused = false;

    public List<WeaponDefinition> AvailableWeapons;
    [SerializeField]
    private int WeaponSpawnCooldown;
    [SerializeField]
    private float NoEnemyRateMultiplier;
    private float weapCooldown;

    private bool canWaveSpawn;
    private int currentWave = 0;

    private bool playedMusic = false;

    private float lastEmergencyWeaponTimeStamp = -1;

    private Dictionary< string, List<Spawner> > spawners;
    private Dictionary<string, int> spawnCount;

    private void Awake()
    {
        spawners = new Dictionary<string, List<Spawner>>() { };
        spawners["ENEMY"] = new List<Spawner>();
        spawners["WEAPON"] = new List<Spawner>();

        spawnCount = new Dictionary<string, int>() { };
        spawnCount["ENEMY"] = 0;
        spawnCount["WEAPON"] = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        weapCooldown = WeaponSpawnCooldown * 60;
        
        gameOverScreen = GameObject.Find("Restart");
        gameOverScreen.SetActive(false);
        player = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (!paused)
            {
                GameObject.Find("Paused").transform.GetChild(0).gameObject.SetActive(true);
                Time.timeScale = 0;
                paused = true;
            }
            else
            {
                GameObject.Find("Paused").transform.GetChild(0).gameObject.SetActive(false);
                Time.timeScale = 1;
                paused = false;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!playedMusic && AudioManager.instance)
        {
            AudioManager.instance.Play("BattleMusic");
            playedMusic = true;
        }
        
        //Game over on player death
        if (player == null)
        {
            gameOverScreen.SetActive(true);
            gameOverScreen.transform.GetChild(0).gameObject.SetActive(true);
            gameOverScreen.transform.GetChild(1).gameObject.SetActive(true);
        }
        else if (player.loadout.IsEmpty() && Time.time >= lastEmergencyWeaponTimeStamp + EmergencyWeaponCooldown)
        {
            lastEmergencyWeaponTimeStamp = Time.time;
            SpawnEntity("WEAPON");
        }

    
        if (WaveEnded())
        {
            cooldown -= 1;
            weapCooldown -= NoEnemyRateMultiplier;
        } else {
            weapCooldown -=1;
        }
        canWaveSpawn = cooldown <= 0;
        

        if (weapCooldown <= 0){
            SpawnEntity("WEAPON");
            weapCooldown = WeaponSpawnCooldown *60;
        }

        if (canWaveSpawn)
        {
            EnemyWaveSpawn();
        }

        
    }

    private bool WaveEnded(){
        return spawnCount["ENEMY"] <= 0;
        
    }
    //activates many spawners at once
    void EnemyWaveSpawn()
    {
        int waveEnemies = InitialWaveEnemies + WaveEnemyIncrease * currentWave;

        for (int i=0; i<waveEnemies; i++){
            SpawnEntity("ENEMY");
        }
        
        cooldown = WaveCooldownSecs * 60;
        currentWave += 1;
    }
    


    void SpawnEntity(string spawnType){
        //Select Spawner
        int nSpawners = spawners[spawnType].Count;
        int rand = Random.Range(0,nSpawners);
        Spawner spawner = spawners[spawnType][rand];

        spawnCount[spawnType] += 1;

        spawner.Activate();

    }

    public void DecreaseSpawnCount(string SpawnerType) {
        spawnCount[SpawnerType] -= 1;
    }


    //Lets a new spawner (which must have an Activate method), be activated by a Manager.
    //The spawnerType string lets the spawner diferentiate between arbitrarily defines classes of spawner.
    public void SubscribeSpawner(string spawnerType, Spawner spawner)
    {
        spawners[spawnerType].Add(spawner);   
    }

    public void Restart()
    {
        gameOverScreen.SetActive(false);
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
