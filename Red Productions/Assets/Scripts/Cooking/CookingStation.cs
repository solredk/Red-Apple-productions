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


    private bool isInteractionActive = false;
    private float interactionStartTime = 0; // Add this field to track when interaction started
    private float interactionDuration = 1.0f; // Duration to check ingredients (1 second)
    private float lastLogTime = 0;
    private float logCooldown = 0.1f; // Check more frequently, 5x per second

    private void Start()
    {
        if (finalProductSpawnPoint == null)
            finalProductSpawnPoint = transform;
        if (interactionSprite != null)
            interactionSprite.SetActive(false);



        Debug.Log($"<color=cyan>[CookingStation]</color> Initialized with recipe type: {recipeType}");
    }

    private void Update()
    {
        if (isInteractionActive)
        {
            // Check if we're still within the time window
            if (Time.time < interactionStartTime + interactionDuration)
            {
                // Check ingredients periodically during this 1-second window
                if (Time.time > lastLogTime + logCooldown)
                {
                    lastLogTime = Time.time;
                    Debug.LogError($"<color=yellow>[CookingStation]</color> Checking ingredients at {Time.time}, time remaining: {(interactionStartTime + interactionDuration - Time.time):F2}s");
                    CheckIngredientsInRadius();
                }
            }
            else
            {
                // Time window expired, stop checking
                Debug.LogError($"<color=orange>[CookingStation]</color> Ingredient checking duration complete after 1 second.");
                isInteractionActive = false;
            }
        }
    }

    public void ShowInteractionIndicator(bool show)
    {
        if (interactionSprite != null)
            interactionSprite.SetActive(show);
    }

    private void CheckIngredientsInRadius()
    {
        if (ingredientManager == null)
        {
            Debug.LogError("<color=red>[CookingStation]</color> CRITICAL ERROR: No IngredientManager assigned!");
            return;
        }

        // Get the layer and radius from the manager
        int ingredientLayer = ingredientManager.GetIngredientLayer();
        float checkRadius = ingredientManager.GetCheckRadius();

        Debug.LogError($"<color=yellow>[CookingStation]</color> Checking with layer: {ingredientLayer}, radius: {checkRadius}");

        // Find all colliders in the radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, checkRadius, ingredientLayer);
        Debug.LogError($"<color=yellow>[CookingStation]</color> Found {colliders.Length} colliders in radius");

        // Count each type of ingredient
        Dictionary<Ingredient.Ingredients, int> foundIngredients = new Dictionary<Ingredient.Ingredients, int>();
        foreach (Collider collider in colliders)
        {
            Ingredient ingredient = collider.GetComponent<Ingredient>();
            if (ingredient != null)
            {
                Debug.LogError($"<color=green>[CookingStation]</color> Found ingredient: {ingredient.ingredients} in {collider.name}");

                if (!foundIngredients.ContainsKey(ingredient.ingredients))
                    foundIngredients[ingredient.ingredients] = 0;

                foundIngredients[ingredient.ingredients]++;
            }
        }

        // Log what we found
        foreach (var item in foundIngredients)
        {
            Debug.LogError($"<color=green>[CookingStation]</color> Ingredient count: {item.Key} = {item.Value}");
        }

        // Use the manager to check if we have all requirements
        bool hasAllIngredients = ingredientManager.CheckIngredientsInRadius(recipeType, transform.position);
        Debug.LogError($"<color=yellow>[CookingStation]</color> Has all ingredients: {hasAllIngredients}");
    }

    // Override the Interact method from the Interactable base class
    protected override void Interact()
    {
        base.Interact();

        Debug.LogError($"<color=red>[CookingStation]</color> INTERACT CALLED on {gameObject.name}");

        if (!isInteractionActive)
        {
            isInteractionActive = true;
            interactionStartTime = Time.time; // Record when we started
            lastLogTime = 0; // Reset log timer to ensure immediate first check
            Debug.LogError($"<color=green>[CookingStation]</color> Starting ingredient check for {recipeType} (1-second duration)");

            if (ingredientManager != null)
            {
                ingredientManager.startIngredientCheck(recipeType, transform.position, this);
                CheckIngredientsInRadius(); // Immediate check
            }
            else
            {
                Debug.LogError($"<color=red>[CookingStation]</color> CRITICAL ERROR: No IngredientManager found!");
                isInteractionActive = false; // Reset interaction flag since we can't proceed
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

    {     // Draw interaction range
        Gizmos.color = new Color(0.2f, 0.5f, 1f, 0.3f); // Semi-transparent blue
        Gizmos.DrawSphere(transform.position, interactionRange);

        // Draw wire sphere for better visibility
        Gizmos.color = new Color(0.2f, 0.5f, 1f, 1f);
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
    
}




