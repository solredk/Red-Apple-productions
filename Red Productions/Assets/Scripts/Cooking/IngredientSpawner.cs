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

        if (string.IsNullOrEmpty(promptMessage))
            promptMessage = $"Press Q to spawn ingredient (Cost: {costToSpawn})";
    }

    private void Update()
    {
        if (!canSpawn)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                canSpawn = true;
                cooldownTimer = 0f;
            }
        }
    }

    public void ShowInteractionIndicator(bool show)
    {
        if (interactionSprite != null)
            interactionSprite.SetActive(show);
    }

    protected override void Interact(GameObject playerGameObject)
    {
        if (!canSpawn)
            return;

        if (ScoreSystem.Instance != null && ScoreSystem.Instance.score >= costToSpawn)
        {
            ScoreSystem.Instance.AddScore(-costToSpawn);

            if (ingredientPrefab != null && spawnPoint != null)
            {
                GameObject spawned = Instantiate(ingredientPrefab, spawnPoint.position, spawnPoint.rotation);

                Rigidbody rb = spawned.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(Vector3.up * 2f + Random.insideUnitSphere * 1f, ForceMode.Impulse);
                }
            }

            canSpawn = false;
            cooldownTimer = cooldownTime;
        }
        else
        {
            Debug.LogError("Not enough points to spawn ingredient!");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(spawnPoint.position, 0.2f);
        }
    }
}