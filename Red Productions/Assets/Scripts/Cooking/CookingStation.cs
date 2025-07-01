using System;
using System.Collections.Generic;
using UnityEngine;

public class CookingStation : Interactable, IIngredientCheckListener
{
    [SerializeField] private Ingredient.IngredientType recipeType;
    [SerializeField] private IngredientManager ingredientManager;
    [SerializeField] private Transform finalProductSpawnPoint;
    [SerializeField] private bool destroyIngredientsOnComplete = true;
    [SerializeField] private GameObject interactionSprite;
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private float ingredientCheckTimeout = 5f; // Time to wait before giving up

    private bool isInteractionActive = false;

    private void Start()
    {
        if (finalProductSpawnPoint == null)
            finalProductSpawnPoint = transform;
        if (interactionSprite != null)
            interactionSprite.SetActive(false);

        Debug.Log($"<color=cyan>[CookingStation]</color> Initialized with recipe type: {recipeType}");
    }

    public void ShowInteractionIndicator(bool show)
    {
        if (interactionSprite != null)
            interactionSprite.SetActive(show);
    }

    // Simplified Interact method that delegates to IngredientManager
    protected override void Interact()
    {
        base.Interact();

        Debug.LogError($"<color=red>[CookingStation]</color> INTERACT CALLED on {gameObject.name}");

        if (!isInteractionActive)
        {
            if (ingredientManager != null)
            {
                isInteractionActive = true;
                Debug.LogError($"<color=green>[CookingStation]</color> Starting ingredient check for {recipeType}");

                // Delegate the ingredient checking to the IngredientManager
                ingredientManager.startIngredientCheck(recipeType, transform.position, this);
            }
            else
            {
                Debug.LogError($"<color=red>[CookingStation]</color> CRITICAL ERROR: No IngredientManager found!");
            }
        }
        else
        {
            Debug.LogError($"<color=orange>[CookingStation]</color> Already checking ingredients!");
        }
    }

    public void OnIngredientsReady()
    {
        Debug.LogError($"<color=green>[CookingStation]</color> ALL INGREDIENTS FOUND! Recipe complete!");

        // Spawn the final product
        List<GameObject> createdFoodItems = ingredientManager.InstantiateIngredientGroup(recipeType, finalProductSpawnPoint.position, finalProductSpawnPoint.rotation);

        if (createdFoodItems != null && createdFoodItems.Count > 0)
        {
            Debug.LogError($"<color=blue>[CookingStation]</color> Spawned {createdFoodItems.Count} food items");

            // Destroy ingredients if configured to do so
            if (destroyIngredientsOnComplete)
            {
                DestroyIngredientsInRadius();
            }
        }
        else
        {
            Debug.LogError($"<color=red>[CookingStation]</color> Failed to spawn any food items!");
        }

        isInteractionActive = false;
    }

    // Add this method to handle missing ingredients
    public void OnIngredientsMissing()
    {
        Debug.LogError($"<color=orange>[CookingStation]</color> MISSING INGREDIENTS for recipe {recipeType}!");
        isInteractionActive = false;
    }

    private void DestroyIngredientsInRadius()
    {
        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            ingredientManager.GetCheckRadius(),
            ingredientManager.GetIngredientLayer());

        int destroyedCount = 0;
        foreach (Collider collider in colliders)
        {
            Ingredient ingredient = collider.GetComponent<Ingredient>();
            if (ingredient != null)
            {
                destroyedCount++;
                Destroy(collider.gameObject);
            }
        }

        Debug.LogError($"<color=magenta>[CookingStation]</color> Destroyed {destroyedCount} ingredients");
    }

    private void OnDrawGizmosSelected()
    {
        // Draw interaction range
        Gizmos.color = new Color(0.2f, 0.5f, 1f, 0.3f); // Semi-transparent blue
        Gizmos.DrawSphere(transform.position, interactionRange);

        // Draw wire sphere for better visibility
        Gizmos.color = new Color(0.2f, 0.5f, 1f, 1f);
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}