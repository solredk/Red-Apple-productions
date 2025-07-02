using UnityEngine;
using System.Collections.Generic;

public class IngredientShopUI : MonoBehaviour
{
    [Header("Manager Reference")]
    [SerializeField] private IngredientManager manager; // Assign in Inspector

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;   // Assign in Inspector
    [SerializeField] private float clearSpawnRadius = 0.5f;

    [Header("Food Costs")]
    [SerializeField] private int burgerCost = 65;
    [SerializeField] private int nuggetCost = 35;
    [SerializeField] private int milkshakeCost = 15;
    [SerializeField] private int friesCost = 40;

    // Example UI Button methods

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

    // Generic spawn method
    private void TrySpawn(Ingredient.IngredientType foodType, int cost)
    {
        // Check score
        if (ScoreSystem.Instance != null && ScoreSystem.Instance.score >= cost)
        {
            ScoreSystem.Instance.AddScore(-cost);

            // Find a clear spawn point
            Transform spawnPoint = FindClearSpawnPoint();
            if (spawnPoint == null && spawnPoints.Length > 0)
            {
                spawnPoint = spawnPoints[0];
                Debug.Log("<color=yellow>[IngredientShopUI]</color> All spawn points blocked, using the first point.");
            }

            // Spawn the food group using IngredientManager
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

    // Attempts to find a spawn point that has no colliders within clearSpawnRadius
    private Transform FindClearSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("<color=red>[IngredientShopUI]</color> No spawn points assigned!");
            return null;
        }

        // Create a random order of indices
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

        // Check each spawn point in random order
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

        // None clear
        return null;
    }

    // For reference or debugging in the scene view
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