using System;
using System.Collections.Generic;
using UnityEngine;

public class CookingStation : MonoBehaviour, IIngredientCheckListener
{
    [SerializeField] private Ingredient.IngredientType recipeType;
    [SerializeField] private IngredientManager ingredientManager;
    [SerializeField] private Transform finalProductSpawnPoint;
    [SerializeField] private bool destroyIngredientsOnComplete = true;

    private bool isInteractionActive = false;
    private float lastLogTime = 0;
    private float logCooldown = 1.0f; // Only log ingredient detection every second

    private void Start()
    {
        if (finalProductSpawnPoint == null)
            finalProductSpawnPoint = transform;

        Debug.Log($"<color=cyan>[CookingStation]</color> Initialized with recipe type: {recipeType}");
    }

    private void Update()
    {
        // Only run detection logging if we're actively checking for ingredients
        if (isInteractionActive && Time.time > lastLogTime + logCooldown)
        {
            lastLogTime = Time.time;
            LogIngredientsInRadius();
        }
    }

    private void LogIngredientsInRadius()
    {
        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            ingredientManager.GetCheckRadius(),
            ingredientManager.GetIngredientLayer());

        if (colliders.Length > 0)
        {
            Dictionary<Ingredient.Ingredients, int> foundIngredients = new Dictionary<Ingredient.Ingredients, int>();

            foreach (Collider collider in colliders)
            {
                Ingredient ingredient = collider.GetComponent<Ingredient>();
                if (ingredient != null)
                {
                    if (!foundIngredients.ContainsKey(ingredient.ingredients))
                        foundIngredients[ingredient.ingredients] = 0;

                    foundIngredients[ingredient.ingredients]++;
                }
            }

            string ingredientsLog = "";
            foreach (var pair in foundIngredients)
            {
                ingredientsLog += $"{pair.Key}: {pair.Value}, ";
            }

            Debug.Log($"<color=yellow>[CookingStation]</color> Ingredients in radius: {ingredientsLog.TrimEnd(',', ' ')}");

            // Check if we have all required ingredients
            bool hasAllIngredients = ingredientManager.CheckIngredientsInRadius(recipeType, transform.position);
            Debug.Log($"<color=yellow>[CookingStation]</color> All required ingredients present: {hasAllIngredients}");
        }
    }

    public void Interact()
    {
        if (!isInteractionActive)
        {
            isInteractionActive = true;

            Debug.Log($"<color=green>[CookingStation]</color> Player interacted with {gameObject.name}");
            Debug.Log($"<color=green>[CookingStation]</color> Starting check for {recipeType} ingredients at position {transform.position}");

            // Start checking for ingredients in radius after interaction
            ingredientManager.startIngredientCheck(recipeType, transform.position, this);

            // Initial check to see what ingredients are present right away
            LogIngredientsInRadius();
        }
        else
        {
            Debug.Log($"<color=orange>[CookingStation]</color> Already checking for ingredients! Please wait...");
        }
    }

    // This implements the IIngredientCheckListener interface
    public void OnIngredientsReady()
    {
        Debug.Log($"<color=green>[CookingStation]</color> All ingredients for {recipeType} found! Recipe complete!");

        // Spawn the final product using the IngredientManager's InstantiateIngredientGroup method
        List<GameObject> createdFoodItems = ingredientManager.InstantiateIngredientGroup(recipeType, finalProductSpawnPoint.position, finalProductSpawnPoint.rotation);

        if (createdFoodItems != null && createdFoodItems.Count > 0)
        {
            Debug.Log($"<color=blue>[CookingStation]</color> Spawned {createdFoodItems.Count} final product(s) for {recipeType} at {finalProductSpawnPoint.position}");
            foreach (GameObject item in createdFoodItems)
            {
                Debug.Log($"<color=blue>[CookingStation]</color> Created food item: {item.name}");
            }
        }
        else
        {
            Debug.LogWarning($"<color=red>[CookingStation]</color> Failed to spawn any food items for {recipeType}!");
        }

        // Destroy ingredients if configured to do so
        if (destroyIngredientsOnComplete)
        {
            DestroyIngredientsInRadius();
        }

        isInteractionActive = false;
        Debug.Log($"<color=cyan>[CookingStation]</color> Ready for next cooking task!");
    }

    private void DestroyIngredientsInRadius()
    {
        Debug.Log($"<color=magenta>[CookingStation]</color> Destroying ingredients in radius {ingredientManager.GetCheckRadius()}");

        // Use the same radius and layer mask as the ingredient manager
        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            ingredientManager.GetCheckRadius(),
            ingredientManager.GetIngredientLayer());

        int destroyedCount = 0;
        List<string> destroyedIngredients = new List<string>();

        foreach (Collider collider in colliders)
        {
            Ingredient ingredient = collider.GetComponent<Ingredient>();
            if (ingredient != null)
            {
                destroyedIngredients.Add(ingredient.ingredients.ToString());
                destroyedCount++;
                Destroy(collider.gameObject);
            }
        }

        Debug.Log($"<color=magenta>[CookingStation]</color> Destroyed {destroyedCount} ingredients: {string.Join(", ", destroyedIngredients)}");
    }

    private void OnDrawGizmosSelected()
    {
        if (ingredientManager != null)
        {
            Gizmos.color = Color.yellow;
            float radius = ingredientManager.GetCheckRadius();
            Gizmos.DrawWireSphere(transform.position, radius);

            // This only appears in the Scene view but can help during development
#if UNITY_EDITOR
            UnityEditor.Handles.Label(transform.position + Vector3.up * radius,
                $"Recipe: {recipeType}\nRadius: {radius}");
#endif
        }
    }
}