using UnityEngine;

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

    [Header("Spawn Count")]
    [SerializeField] private int minIngredientsPerSpawn = 1;
    [SerializeField] private int maxIngredientsPerSpawn = 3;

    private bool canSpawn = true;
    private float cooldownTimer = 0f;

    public float InteractionRange => interactionRange; // Property to access the range from other scripts

    private void Start()
    {
        // If no spawn points are defined, use this transform as default
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            spawnPoints = new Transform[1];
            spawnPoints[0] = transform;
        }

        if (interactionSprite != null)
            interactionSprite.SetActive(false);

        // Set a default prompt message if not provided
        if (string.IsNullOrEmpty(promptMessage))
            promptMessage = $"Press Q to spawn ingredients (Cost: {costToSpawn})";
    }

    private void Update()
    {
        // Handle cooldown
        if (!canSpawn)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0)
            {
                canSpawn = true;
                cooldownTimer = 0;
            }
        }
    }

    public void ShowInteractionIndicator(bool show)
    {
        if (interactionSprite != null)
            interactionSprite.SetActive(show);
    }

    protected override void Interact()
    {
        Debug.LogError($"<color=blue>[IngredientSpawner]</color> Interact called on {gameObject.name}");

        if (!canSpawn)
        {
            Debug.LogError($"<color=orange>[IngredientSpawner]</color> Spawner on cooldown! Please wait.");
            return;
        }

        // Check if we have enough score
        if (ScoreSystem.Instance != null && ScoreSystem.Instance.score >= costToSpawn)
        {
            // Deduct points
            ScoreSystem.Instance.AddScore(-costToSpawn);

            // Determine how many ingredients to spawn
            int spawnCount = Random.Range(minIngredientsPerSpawn, maxIngredientsPerSpawn + 1);

            // Spawn multiple ingredients
            for (int i = 0; i < spawnCount; i++)
            {
                SpawnIngredient();
            }

            Debug.LogError($"<color=green>[IngredientSpawner]</color> Spawned {spawnCount} ingredients and deducted {costToSpawn} points");

            // Set cooldown
            canSpawn = false;
            cooldownTimer = cooldownTime;
        }
        else
        {
            Debug.LogError($"<color=red>[IngredientSpawner]</color> Not enough points! Need {costToSpawn} points to spawn.");
        }
    }

    private void SpawnIngredient()
    {
        // Check if we have prefabs to spawn
        if (ingredientPrefabs == null || ingredientPrefabs.Length == 0)
        {
            Debug.LogError("<color=red>[IngredientSpawner]</color> No ingredient prefabs assigned!");
            return;
        }

        // Randomly select a prefab and spawn point
        GameObject selectedPrefab = ingredientPrefabs[Random.Range(0, ingredientPrefabs.Length)];
        Transform selectedSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Instantiate the ingredient at the selected spawn point
        GameObject spawnedIngredient = Instantiate(selectedPrefab, selectedSpawnPoint.position, selectedSpawnPoint.rotation);

        // Apply force to the rigidbody with slight variation for each ingredient
        Rigidbody rb = spawnedIngredient.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Add some variation to the force to spread ingredients out
            Vector3 randomForce = new Vector3(
                Random.Range(0.5f, 1.0f),
                Random.Range(0.5f, 1.0f),
                Random.Range(0.5f, 1.0f));

            rb.AddForce(randomForce + Random.insideUnitSphere * 1f, ForceMode.Impulse);
        }
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
                    Gizmos.DrawWireSphere(point.position, 0.3f);
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