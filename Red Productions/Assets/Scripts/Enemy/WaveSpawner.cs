using System.Collections.Generic;
using UnityEngine;
using System.Collections;

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
            NextWave();
    }

    public IEnumerator SpawnLoop()
    {
        if (players.Count == 0)
            players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));

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
        zombiesKilled = 0;
        waveRequirments = Mathf.CeilToInt(waveRequirments * 1.01f);

        enemyBehavior.currentWave++;

        enemyBehavior.reward += 5;

        if (enemyBehavior.attackCooldown > 0.3f)
            enemyBehavior.attackCooldown *= 0.95f;

        enemyBehavior.maxhealth = Mathf.CeilToInt(enemyBehavior.maxhealth * 1.2f);

        enemyBehavior.damage = Mathf.CeilToInt(enemyBehavior.damage * 1.15f);

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

    private void OnDrawGizmosSelected()
    {
        // Spawn area
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);

        // No spawn zone
        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);


        // Jump in points
        if (jumpInPoints != null)
        {
            Gizmos.color = Color.green;
            foreach (var t in jumpInPoints)
            {
                if (t != null)
                    Gizmos.DrawSphere(t.position, 0.5f);
            }
        }
    }
}