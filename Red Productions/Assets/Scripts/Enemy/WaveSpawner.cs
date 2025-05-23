using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
public class WaveSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject zombiePrefab;

    [SerializeField] private int maxZombies = 10;
    public int zombiesKilled;

    private int waveRequirments = 10;

    [SerializeField] private float spawnIntervalMin = 1f;
    [SerializeField] private float spawnIntervalMax = 10f;

    [Header("Spawn Area (World Space)")]
    [SerializeField] private Vector3 spawnAreaCenter;
    [SerializeField] private Vector3 spawnAreaSize;

    [SerializeField] private Enemybehavior enemyBehavior;
    
    private List<GameObject> spawnedZombies = new List<GameObject>();

    private int currentZombies = 0;

    [SerializeField] private List<GameObject> players;
    [SerializeField] private float safeDistanceFromPlayer = 1.5f;


    [Header("Zombie StartStats")]
    [SerializeField] private int startWave;
    [SerializeField] private int startDamage;
    [SerializeField] private int startMaxhealth;
    [SerializeField] private float startAttackCooldown;
    [SerializeField] private float startSpeed;

    private void Awake()
    {
        SetZombieStartStats();
    }

    private void Update()
    {
        if (zombiesKilled > waveRequirments)
            NextWave();
    }


    public IEnumerator SpawnLoop()
    {
        if (players.Count == 0)
            players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        
        while (true)
        {
            // Wait for a random interval before spawning the next zombie
            yield return new WaitForSeconds(Random.Range(spawnIntervalMin, spawnIntervalMax));
            
  
            spawnedZombies.RemoveAll(z => z == null);

            // putting the current number of zombies in en seprate variable
            currentZombies = spawnedZombies.Count;

            if (currentZombies < maxZombies)
            {
                //get the new spawn position and instantiate the zombie and add it to the list
                Vector3 spawnPos = GetRandomPositionInArea();
                GameObject newZombie = Instantiate(zombiePrefab, spawnPos, Quaternion.identity);

                spawnedZombies.Add(newZombie);
                currentZombies++;
            }
        }
    }

    private void NextWave()
    {
        zombiesKilled = 0;
        waveRequirments = Mathf.CeilToInt(waveRequirments * 1.2f);

        enemyBehavior.currentWave++;

        if (enemyBehavior.attackCooldown > 0.3f)
            enemyBehavior.attackCooldown *= 0.95f; 


        enemyBehavior.maxhealth = Mathf.CeilToInt(enemyBehavior.maxhealth * 1.2f);

        enemyBehavior.damage = Mathf.CeilToInt(enemyBehavior.damage * 1.15f);

        if (enemyBehavior.speed < 10f)
            enemyBehavior.speed *= 1.05f;
    }


    private Vector3 GetRandomPositionInArea()
    {
        Vector3 spawnPos;
        bool validPosition = false;

        int maxAttempts = 50; // failsafe om infinite loop te voorkomen
        int attempts = 0;

        do
        {
            Vector3 randomOffset = new(
                Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                0,
                Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
            );

            spawnPos = spawnAreaCenter + randomOffset;
            validPosition = true;

            foreach (GameObject player in players)
            {
                if (player == null) continue;

                float distance = Vector3.Distance(spawnPos, player.transform.position);
                if (distance < safeDistanceFromPlayer)
                {
                    validPosition = false;
                    break;
                }
            }

            attempts++;

        } while (!validPosition && attempts < maxAttempts);

        return spawnPos;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
    }

    private void SetZombieStartStats()
    {
        //reset the wave to 1 
        enemyBehavior.currentWave = startWave;        
        
        enemyBehavior.maxhealth = startMaxhealth;

        enemyBehavior.damage = startDamage;
        enemyBehavior.attackCooldown = startAttackCooldown;

        enemyBehavior.speed = startSpeed;
    }
}
