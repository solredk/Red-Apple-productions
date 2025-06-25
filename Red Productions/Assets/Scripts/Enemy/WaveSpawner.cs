using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class WaveSpawner : MonoBehaviour
{
    [Header("the zombie prefab")]
    [SerializeField] private GameObject zombiePrefab;
    [Header("the maximum amount of zombies that can be spawned at once")]
    [SerializeField] private int maxZombies = 10;
    
    // the ammount of zombies that have been killed in the current wave
    private int zombiesKilled;

    //the amount of zombies that need to be killed to start the next wave
    private int waveRequirments = 10;

    [Header("the time in between te zombies will randomly spawn")]
    [SerializeField] private float spawnIntervalMin = 1f;
    [SerializeField] private float spawnIntervalMax = 10f;

    [SerializeField] private Enemybehavior enemyBehavior;

    private List<GameObject> spawnedZombies = new List<GameObject>();

    private int currentZombies = 0;

    [Header("Jump In Points")]
    [SerializeField] private List<Transform> jumpInPoints;

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
        {
            NextWave();
        }
    }

    public IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(spawnIntervalMin, spawnIntervalMax));

            spawnedZombies.RemoveAll(z => z == null);
            currentZombies = spawnedZombies.Count;

            if (currentZombies < maxZombies)
            {
                Vector3 spawnPos = GetRandomJumpInPosition();

                GameObject newZombie = Instantiate(zombiePrefab, spawnPos, Quaternion.identity);
                spawnedZombies.Add(newZombie);
                currentZombies++;
            }
        }
    }

    private Vector3 GetRandomJumpInPosition()
    {
        int index = Random.Range(0, jumpInPoints.Count);
        return jumpInPoints[index].position;
    }

    private void NextWave()
    {
        // Reset the zombies killed counter
        zombiesKilled = 0;

        // Increase the wave requirements by 1% to make it harder
        waveRequirments = Mathf.CeilToInt(waveRequirments * 1.01f);

        //increase the current wave of the enemy behavior
        enemyBehavior.currentWave++;

        //increase the kill reward
        enemyBehavior.reward += 5;

        //increate the attack interval
        if (enemyBehavior.attackCooldown > 0.3f)
            enemyBehavior.attackCooldown *= 0.95f;

        //increase the zombie health
        enemyBehavior.maxhealth = Mathf.CeilToInt(enemyBehavior.maxhealth * 1.2f);

        //increase the zombie damage
        enemyBehavior.damage = Mathf.CeilToInt(enemyBehavior.damage * 1.15f);

        //increase the zombie speed
        if (enemyBehavior.speed < 10f)
            enemyBehavior.speed *= 1.05f;
    }

    private void SetZombieStartStats()
    {
        enemyBehavior.currentWave = startWave;

        enemyBehavior.reward = 10; 

        enemyBehavior.maxhealth = startMaxhealth;

        enemyBehavior.damage = startDamage;
        enemyBehavior.attackCooldown = startAttackCooldown;

        enemyBehavior.speed = startSpeed;
    }

    public void ZombieKilled()
    {
        zombiesKilled++;
    }
}