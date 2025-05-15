using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
public class WaveSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject zombiePrefab;
    public int maxZombies = 10;
    public float spawnIntervalMin = 1f;
    public float spawnIntervalMax = 10f;

    [Header("Spawn Area (World Space)")]
    public Vector3 spawnAreaCenter;
    public Vector3 spawnAreaSize;

    private int currentZombies = 0;
    private List<GameObject> spawnedZombies = new List<GameObject>();

    [SerializeField] private PlayerInputManager PlayerInputManager;

    private void Start()
    {
        if (PlayerInputManager == null)
        {
            StartCoroutine(SpawnLoop());
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
                Vector3 spawnPos = GetRandomPositionInArea();
                GameObject newZombie = Instantiate(zombiePrefab, spawnPos, Quaternion.identity);
                spawnedZombies.Add(newZombie);
                currentZombies++;
            }
        }
    }

    private Vector3 GetRandomPositionInArea()
    {
        Vector3 randomOffset = new Vector3(
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            0,
            Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
        );
        return spawnAreaCenter + randomOffset;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
    }
}
