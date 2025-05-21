using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
public class WaveSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject zombiePrefab;

    [SerializeField] private int maxZombies = 10;

    [SerializeField] private float spawnIntervalMin = 1f;
    [SerializeField] private float spawnIntervalMax = 10f;

    [Header("Spawn Area (World Space)")]
    [SerializeField] private Vector3 spawnAreaCenter;
    [SerializeField] private Vector3 spawnAreaSize;

    [SerializeField] private PlayerInputManager PlayerInputManager;
    
    private List<GameObject> spawnedZombies = new List<GameObject>();

    private int currentZombies = 0;

    public IEnumerator SpawnLoop()
    {
        while (true)
        {
            // Wait for a random interval before spawning the next zombie
            yield return new WaitForSeconds(Random.Range(spawnIntervalMin, spawnIntervalMax));

            // remove ewery null zombie from the list
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

    private Vector3 GetRandomPositionInArea()
    {
        // Generate a random position within the defined spawn area
        Vector3 randomOffset = new (
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            0,
            Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
        );
        return spawnAreaCenter + randomOffset;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the spawn area in the editor for visualization
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
    }
}
