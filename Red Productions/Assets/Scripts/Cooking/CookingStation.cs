

using System;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class CookingStation : MonoBehaviour, IIngredientCheckListener
{
    [SerializeField] private IngredientManager ingredientManager;
    [SerializeField] private Transform finalProductSpawnPoint;
    [SerializeField] private bool destroyIngredientsOnComplete = true;
    [SerializeField] private Ingredient.IngredientType recipeType;

    private bool isInteractionActive = false;
    private float lastLogTime = 0;
    private float logCooldown = 1.0f; // Only log ingredient detection every second

    private void Start()
    {
        if (finalProductSpawnPoint == null)
            finalProductSpawnPoint = transform;
    }

    public void Interact()
    {
        if (!isInteractionActive)
        {
            isInteractionActive = true;

            // Start checking for ingredients in radius after interaction
            ingredientManager.startIngredientCheck(recipeType, transform.position, this);

            Debug.Log($"Started checking for {recipeType} ingredients");
        }
    }

    // This implements the IIngredientCheckListener interface
    public void OnIngredientsReady()
    {
        Debug.Log($"All ingredients for {recipeType} found! Recipe complete!");

        // Spawn the final product using the IngredientManager's InstantiateIngredientGroup method
        List<GameObject> createdFoodItems = ingredientManager.InstantiateIngredientGroup(recipeType, finalProductSpawnPoint.position, finalProductSpawnPoint.rotation);

        if (createdFoodItems != null && createdFoodItems.Count > 0)
        {
            Debug.Log($"Spawned final product(s) for {recipeType}");
        }

        // Destroy ingredients if configured to do so
        if (destroyIngredientsOnComplete)
        {
            DestroyIngredientsInRadius();
        }

        isInteractionActive = false;
    }

    private void DestroyIngredientsInRadius()
    {
        // Use the same radius and layer mask as the ingredient manager
        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            ingredientManager.GetCheckRadius(),
            ingredientManager.GetIngredientLayer());

        foreach (Collider collider in colliders)
        {
            Ingredient ingredient = collider.GetComponent<Ingredient>();
            if (ingredient != null)
            {
                Destroy(collider.gameObject);
            }
        }

        Debug.Log("Destroyed ingredients in radius");
    }

    private void OnDrawGizmosSelected()
    {
        if (ingredientManager != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, ingredientManager.GetCheckRadius());
        }
    }
}