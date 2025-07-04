using UnityEngine;
using System.Collections.Generic;

public class IngredientSpawner : Interactable
{
    [Header("Spawner Settings")]
    [SerializeField] private GameObject[] ingredientPrefabs; // Array of possible prefabs
    [SerializeField] private Transform[] spawnPoints;        // Array of possible spawn points
    [SerializeField] private int costToSpawn = 5;
    [SerializeField] private float cooldownTime = 2f;
    [SerializeField] private GameObject interactionSprite;  

    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange = 3f;    // How far away the player can interact with this spawner
    [SerializeField] private float clearSpawnRadius = 0.5f;  // Radius to check for clear spawn point

    [Header("Spawn Count")]
    [SerializeField] private int minIngredientsPerSpawn = 1;
    [SerializeField] private int maxIngredientsPerSpawn = 3;

    private bool canSpawn = true;
    private float cooldownEndTime = 0f;

    public float InteractionRange => interactionRange; // Property to access the range from other scripts

    private void Start()
    {

        if (interactionSprite != null)
            interactionSprite.SetActive(false);

        // Set a default prompt message if not provided
        if (string.IsNullOrEmpty(promptMessage))
            promptMessage = $"Press Q to spawn ingredients (Cost: {costToSpawn})";
    }

    private void Update()
    {
        // Handle cooldown more efficiently using Time.time
        if (!canSpawn && Time.time >= cooldownEndTime)
        {
            canSpawn = true;
        }
    }
    public void ShowInteractionIndicator(bool show)
    {
        if (interactionSprite != null)
            interactionSprite.SetActive(show);
    }

    protected override void Interact(GameObject playerGameObject)
    {
        Debug.Log($"<color=blue>[IngredientSpawner]</color> Interact called on {gameObject.name}");

        if (!canSpawn)
        {
            Debug.Log($"<color=orange>[IngredientSpawner]</color> Spawner on cooldown! Please wait {cooldownEndTime - Time.time:F1} seconds.");
            return;
        }

        // Check if we have enough score
        if (ScoreSystem.Instance != null && ScoreSystem.Instance.score >= costToSpawn)
        {
            // Deduct points
            ScoreSystem.Instance.AddScore(-costToSpawn);

            // Determine how many ingredients to spawn
            int spawnCount = Random.Range(minIngredientsPerSpawn, maxIngredientsPerSpawn + 1);
            int successfulSpawns = 0;

            // Spawn multiple ingredients
            for (int i = 0; i < spawnCount; i++)
            {
                if (SpawnIngredient())
                    successfulSpawns++;
            }

            Debug.Log($"<color=green>[IngredientSpawner]</color> Spawned {successfulSpawns} ingredients and deducted {costToSpawn} points");

            // Set cooldown using absolute time - more efficient
            canSpawn = false;
            cooldownEndTime = Time.time + cooldownTime;
        }
        else
        {
            Debug.LogError($"<color=red>[IngredientSpawner]</color> Not enough points! Need {costToSpawn} points to spawn.");
        }
    }
    private bool SpawnIngredient()
    {
        // Check if we have prefabs to spawn
        if (ingredientPrefabs == null || ingredientPrefabs.Length == 0)
        {
            Debug.LogError("<color=red>[IngredientSpawner]</color> No ingredient prefabs assigned!");
            return false;
        }

        GameObject selectedPrefab = ingredientPrefabs[Random.Range(0, ingredientPrefabs.Length)];
        // Try to find a spawn point without colliders
        Transform selectedSpawnPoint = null;

      // shuffle 
        List<int> indices = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
            indices.Add(i);
        for (int i = 0; i < indices.Count; i++)
        {
            int temp = indices[i];
            int randomIndex = Random.Range(i, indices.Count);
            indices[i] = indices[randomIndex];
            indices[randomIndex] = temp;
        }

        foreach (int i in indices)
        {
            // Check if this spawn point is clear (no colliders)
            if (Physics.OverlapSphere(spawnPoints[i].position, clearSpawnRadius).Length == 0)
            {
                selectedSpawnPoint = spawnPoints[i];
                break;
            }
        }
        if (selectedSpawnPoint == null && spawnPoints.Length > 0)
        {
            selectedSpawnPoint = spawnPoints[0];
            Debug.Log("<color=yellow>[IngredientSpawner]</color> All spawn points blocked, using first point.");
        }

        GameObject spawnedIngredient = Instantiate(selectedPrefab, selectedSpawnPoint.position, selectedSpawnPoint.rotation);
        Rigidbody rb = spawnedIngredient.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 randomForce = new Vector3(
                Random.Range(0.5f, 1.0f),
                Random.Range(0.5f, 1.0f),
                Random.Range(0.5f, 1.0f));

            rb.AddForce(randomForce + Random.insideUnitSphere * 1f, ForceMode.Impulse);
        }

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw all spawn points
        if (spawnPoints != null)
        {
            foreach (Transform point in spawnPoints)
            {
                if (point != null)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(point.position, clearSpawnRadius);
                }
            }
        }

        // Draw interaction range
        Gizmos.color = new Color(0.2f, 0.5f, 1f, 0.3f); // Semi-transparent blue
        Gizmos.DrawSphere(transform.position, interactionRange);

        // Draw wire sphere for better visibility
        Gizmos.color = new Color(0.2f, 0.5f, 1f, 1f);
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}