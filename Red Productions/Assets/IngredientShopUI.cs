using System.Collections.Generic;
using UnityEngine;

public class IngredientShopUI : Interactable
{
    [Header("Manager Reference")]
    [SerializeField] private IngredientManager manager;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float clearSpawnRadius = 0.5f;

    [Header("Food Costs")]
    [SerializeField] private int burgerCost = 65;
    [SerializeField] private int nuggetCost = 35;
    [SerializeField] private int milkshakeCost = 15;
    [SerializeField] private int friesCost = 40;

    [Header("UI Settings")]
    [Tooltip("Assign your regular shop UI canvas or panel here")]
    [SerializeField] private GameObject shopHolder;

    private PlayerMovement currentPlayerMovement;
    private PlayerHealth currentPlayerHealth;

    private bool isShopOpen;
    private float lastKnownHealth;

    protected override void Interact(GameObject playerGameObject)
    {
        base.Interact(playerGameObject);

        currentPlayerMovement = playerGameObject.GetComponent<PlayerMovement>();
        currentPlayerHealth = playerGameObject.GetComponent<PlayerHealth>();

        OpenShopUI();
    }

    public void OpenShopUI()
    {
        if (!isShopOpen)
        {
            if (shopHolder != null)
            {
                shopHolder.SetActive(true);
            }

            if (currentPlayerMovement != null)
            {
                currentPlayerMovement.SetCurrentSpeed(0f);
            }

            if (currentPlayerHealth != null)
            {
                lastKnownHealth = currentPlayerHealth.currentHealth;
            }

            isShopOpen = true;
        }
    }

    public void CloseShopUI()
    {
        if (isShopOpen)
        {
            if (shopHolder != null)
            {
                shopHolder.SetActive(false);
            }

            if (currentPlayerMovement != null)
            {
                currentPlayerMovement.SetCurrentSpeed(5f);
            }

            isShopOpen = false;
        }
    }

    private void Update()
    {
        if (isShopOpen && currentPlayerHealth && currentPlayerHealth.currentHealth < lastKnownHealth)
        {
            Debug.Log("Player took damage while in shop, closing UI");
            CloseShopUI();
        }
    }

    public void OnBurgerClick()
    {
        TrySpawn(Ingredient.IngredientType.burger, burgerCost);
    }

    public void OnChickenNuggetClick()
    {
        TrySpawn(Ingredient.IngredientType.chickenNuggets, nuggetCost);
    }

    public void OnMilkshakeClick()
    {
        TrySpawn(Ingredient.IngredientType.milkShakes, milkshakeCost);
    }

    public void OnFriesClick()
    {
        TrySpawn(Ingredient.IngredientType.fries, friesCost);
    }

    private void TrySpawn(Ingredient.IngredientType foodType, int cost)
    {
        if (ScoreSystem.Instance != null && ScoreSystem.Instance.score >= cost)
        {
            ScoreSystem.Instance.AddScore(-cost);

            Transform spawnPoint = FindClearSpawnPoint();
            if (spawnPoint == null && spawnPoints.Length > 0)
            {
                spawnPoint = spawnPoints[0];
                Debug.Log("<color=yellow>[IngredientShopUI]</color> All spawn points blocked, using the first point.");
            }

            if (spawnPoint != null && manager != null)
            {
                manager.InstantiateIngredientGroup(foodType, spawnPoint.position, spawnPoint.rotation);
            }
        }
        else
        {
            Debug.LogError("Not enough points to buy this item!");
        }
    }

    private Transform FindClearSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("<color=red>[IngredientShopUI]</color> No spawn points assigned!");
            return null;
        }

        List<int> indices = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            indices.Add(i);
        }

        for (int i = 0; i < indices.Count; i++)
        {
            int temp = indices[i];
            int randomIndex = Random.Range(i, indices.Count);
            indices[i] = indices[randomIndex];
            indices[randomIndex] = temp;
        }

        foreach (int index in indices)
        {
            Transform point = spawnPoints[index];
            if (point != null)
            {
                Collider[] colliders = Physics.OverlapSphere(point.position, clearSpawnRadius);
                if (colliders.Length == 0)
                {
                    return point;
                }
            }
        }
        return null;
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnPoints != null)
        {
            Gizmos.color = Color.green;
            foreach (Transform spawnPoint in spawnPoints)
            {
                if (spawnPoint != null)
                {
                    Gizmos.DrawWireSphere(spawnPoint.position, clearSpawnRadius);
                }
            }
        }
    }
}