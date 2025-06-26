using UnityEngine;

public class IngredientSpawner : Interactable
{
    [Header("Spawner Settings")]
    [SerializeField] private GameObject ingredientPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int costToSpawn = 5;
    [SerializeField] private float cooldownTime = 2f;
    [SerializeField] private GameObject interactionSprite;

    private bool canSpawn = true;
    private float cooldownTimer = 0f;

    private void Start()
    {
        if (spawnPoint == null)
            spawnPoint = transform;

        if (interactionSprite != null)
            interactionSprite.SetActive(false);

     
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
        base.Interact();
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

            // Spawn the ingredient
            SpawnIngredient();

            // Set cooldown
            canSpawn = false;
            cooldownTimer = cooldownTime;

            Debug.LogError($"<color=green>[IngredientSpawner]</color> Spawned ingredient and deducted {costToSpawn} points");
        }
        else
        {
            Debug.LogError($"<color=red>[IngredientSpawner]</color> Not enough points! Need {costToSpawn} points to spawn.");
        }
    }

    private void SpawnIngredient()
    {
        if (ingredientPrefab == null)
        {
            Debug.LogError("<color=red>[IngredientSpawner]</color> No ingredient prefab assigned!");
            return;
        }

        // Instantiate the ingredient at the spawn point
        GameObject spawnedIngredient = Instantiate(ingredientPrefab, spawnPoint.position, spawnPoint.rotation);

        Rigidbody rb = spawnedIngredient.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(Vector3.up * 2f + Random.insideUnitSphere * 1f, ForceMode.Impulse);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(spawnPoint.position, 0.3f);
        }
    }
}