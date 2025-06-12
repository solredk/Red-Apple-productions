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

    [Header("No Spawn Zone")]
    [SerializeField] private Vector3 noSpawnZoneCenter;
    [SerializeField] private Vector3 noSpawnZoneSize;

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

                // Veiligheid check: spawnPos mag NIET in noSpawnZone of te dichtbij speler zijn
                if (!IsPointInsideBox(spawnPos, noSpawnZoneCenter, noSpawnZoneSize) &&
                    IsSafeDistanceFromPlayers(spawnPos))
                {
                    GameObject newZombie = Instantiate(zombiePrefab, spawnPos, Quaternion.identity);
                    spawnedZombies.Add(newZombie);
                    currentZombies++;
                }
                // Anders skip spawn deze keer, volgende keer nieuwe positie
            }
        }
    }

    private Vector3 GetRandomJumpInPosition()
    {
        if (jumpInPoints.Count == 0)
            return GetRandomPositionInArea(); // fallback

        int index = Random.Range(0, jumpInPoints.Count);
        return jumpInPoints[index].position;
    }

    private bool IsSafeDistanceFromPlayers(Vector3 position)
    {
        foreach (GameObject player in players)
        {
            if (player == null) continue;
            float distance = Vector3.Distance(position, player.transform.position);
            if (distance < safeDistanceFromPlayer)
                return false;
        }
        return true;
    }

    private Vector3 GetRandomPositionInArea()
    {
        Vector3 spawnPos;
        bool validPosition = false;

        int maxAttempts = 50;
        int attempts = 0;

        do
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                0,
                Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
            );

            spawnPos = spawnAreaCenter + randomOffset;
            validPosition = true;

            if (IsPointInsideBox(spawnPos, noSpawnZoneCenter, noSpawnZoneSize))
                validPosition = false;

            if (validPosition && !IsSafeDistanceFromPlayers(spawnPos))
                validPosition = false;

            attempts++;

        } while (!validPosition && attempts < maxAttempts);

        return spawnPos;
    }

    private bool IsPointInsideBox(Vector3 point, Vector3 boxCenter, Vector3 boxSize)
    {
        Vector3 min = boxCenter - boxSize / 2f;
        Vector3 max = boxCenter + boxSize / 2f;

        return (point.x >= min.x && point.x <= max.x) &&
               (point.y >= min.y && point.y <= max.y) &&
               (point.z >= min.z && point.z <= max.z);
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

    private void SetZombieStartStats()
    {
        enemyBehavior.currentWave = startWave;

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
        Gizmos.DrawCube(noSpawnZoneCenter, noSpawnZoneSize);

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