using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CookingStation : Interactable, IIngredientCheckListener
{
    [SerializeField] private Ingredient.IngredientType recipeType;
    [SerializeField] private IngredientManager ingredientManager;
    [SerializeField] private Transform finalProductSpawnPoint;
    [SerializeField] private bool destroyIngredientsOnComplete = true;
    [SerializeField] private GameObject interactionSprite;
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private float ingredientCheckTimeout = 5f;

    // Called when cooking begins (e.g. to trigger a UI or sound)
    [SerializeField] private UnityEvent onCookingStarted;
    // Called when cooking finishes
    [SerializeField] private UnityEvent onCookingEnded;

    // Example extra positions for dropped ingredients (set in the Inspector)
    [SerializeField] private Transform[] droppedIngredientSpots;

    [Header("Drop Point Settings")]
    [SerializeField] private float dropPointRange = 1.5f; // Adjustable in Inspector

    private bool isInteractionActive = false;
    private bool isCookingCooldown = false;

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
       // Drop Point Logic
        if (!isCookingCooldown && droppedIngredientSpots != null && droppedIngredientSpots.Length > 0)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, dropPointRange);
            int spotIndex = 0;
            foreach (Collider col in colliders)
            {
                Ingredient ingredient = col.GetComponent<Ingredient>();
                if (ingredient != null && spotIndex < droppedIngredientSpots.Length)
                {
                    // Snap ingredient to drop spot if not already parented
                    if (ingredient.transform.parent != droppedIngredientSpots[spotIndex])
                    {
                        ingredient.transform.SetParent(droppedIngredientSpots[spotIndex], true);
                        ingredient.transform.localPosition = Vector3.zero;
                        ingredient.transform.localRotation = Quaternion.identity;
                    }
                    spotIndex++;
                    if (spotIndex >= droppedIngredientSpots.Length)
                        break;
                }
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
        base.Interact(playerGameObject);

        Debug.LogError($"<color=red>[CookingStation]</color> INTERACT CALLED on {gameObject.name}");

        if (!isInteractionActive)
        {
            if (ingredientManager != null)
            {
                isInteractionActive = true;
                Debug.LogError($"<color=green>[CookingStation]</color> Starting ingredient check for {recipeType}");
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

    // Called by IngredientManager when all required ingredients are detected
    public void OnIngredientsReady()
    {
        StartCoroutine(CookingCooldownRoutine());
    }

    // Called by IngredientManager if ingredients are missing after the check
    public void OnIngredientsMissing()
    {
        Debug.LogError($"<color=orange>[CookingStation]</color> MISSING INGREDIENTS for recipe {recipeType}!");
        isInteractionActive = false;
    }

    // Runs a 5-second cooldown, then spawns final product
    private IEnumerator CookingCooldownRoutine()
    {
        isCookingCooldown = true;
        onCookingStarted?.Invoke();

        yield return new WaitForSeconds(5f); // Example 5-second cooking time

        // Spawn final product
        Debug.LogError($"<color=green>[CookingStation]</color> ALL INGREDIENTS FOUND! Recipe complete!");

        List<GameObject> createdFoodItems = ingredientManager.InstantiateIngredientGroup(
            recipeType, finalProductSpawnPoint.position, finalProductSpawnPoint.rotation);

        if (createdFoodItems != null && createdFoodItems.Count > 0)
        {
            Debug.LogError($"<color=blue>[CookingStation]</color> Spawned {createdFoodItems.Count} food items");
            if (destroyIngredientsOnComplete)
            {
                DestroyIngredientsInRadius();
            }
        }
        else
        {
            Debug.LogError($"<color=red>[CookingStation]</color> Failed to spawn any food items!");
        }

        onCookingEnded?.Invoke();
        isCookingCooldown = false;
        isInteractionActive = false;
    }


    public bool TryPlaceDroppedIngredient(Ingredient droppedIng, GameObject playerGameObject)
    {
        if (!isCookingCooldown)
            return false;

        // Only allow required ingredients for this recipe
        if (ingredientManager != null &&
            ingredientManager.groupDictionary.TryGetValue(recipeType, out var group) &&
            !group.requiredIngredients.ContainsKey(droppedIng.ingredients))
        {
            // Not a required ingredient for this recipe
            return false;
        }

        // Remove from player's hand if they are holding this ingredient
        if (playerGameObject != null)
        {
            Pickup pickup = playerGameObject.GetComponent<Pickup>();
            if (pickup != null && pickup.inHandItem == droppedIng.gameObject)
            {
                // Detach from any parent first
                droppedIng.transform.SetParent(null, true);
                pickup.inHandItem = null;
                pickup.isHolding = false;
                pickup.tomatoWeapon.SetActive(true);
            }
        }

        Rigidbody rb = droppedIng.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
        }

        // Set the dropped ingredient as a child of this CookingStation GameObject
        droppedIng.transform.SetParent(this.transform, true);

        // Optionally, snap to a drop spot if available
        if (droppedIngredientSpots != null && droppedIngredientSpots.Length > 0)
        {
            Transform spot = droppedIngredientSpots[0];
            droppedIng.transform.position = spot.position;
            droppedIng.transform.rotation = spot.rotation;
        }

        return true;
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
        // Draw drop point range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, dropPointRange);

        // Draw all drop spots
        if (droppedIngredientSpots != null)
        {
            Gizmos.color = Color.green;
            foreach (Transform spot in droppedIngredientSpots)
            {
                if (spot != null)
                    Gizmos.DrawWireSphere(spot.position, 0.15f);
            }
        }

        // Draw interaction range
        Gizmos.color = new Color(0.2f, 0.5f, 1f, 0.3f);
        Gizmos.DrawSphere(transform.position, interactionRange);

        Gizmos.color = new Color(0.2f, 0.5f, 1f, 1f);
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}