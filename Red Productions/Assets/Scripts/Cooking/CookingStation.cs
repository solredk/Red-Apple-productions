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

    [SerializeField] private UnityEvent InteractionFailed;
    [SerializeField] private UnityEvent onCookingStarted;
    [SerializeField] private UnityEvent onCookingEnded;

    [SerializeField] private Transform[] droppedIngredientSpots;

    [Header("Drop Point Settings")]
    [SerializeField] private float dropPointRange = 1.5f;

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
        if (!isCookingCooldown && droppedIngredientSpots != null && droppedIngredientSpots.Length > 0)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, dropPointRange);
            int spotIndex = 0;
            foreach (Collider col in colliders)
            {
                Ingredient ingredient = col.GetComponent<Ingredient>();
                if (ingredient != null && spotIndex < droppedIngredientSpots.Length)
                {
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
        DestroyIngredientsInRadius(); // Destroy ingredients immediately when cooking starts
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

        yield return new WaitForSeconds(5f);

        Debug.LogError($"<color=green>[CookingStation]</color> ALL INGREDIENTS FOUND! Recipe complete!");

        List<GameObject> createdFoodItems = ingredientManager.InstantiateIngredientGroup(
            recipeType, finalProductSpawnPoint.position, finalProductSpawnPoint.rotation);

        if (createdFoodItems != null && createdFoodItems.Count > 0)
        {
            Debug.LogError($"<color=blue>[CookingStation]</color> Spawned {createdFoodItems.Count} food items");
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

        if (ingredientManager != null &&
            ingredientManager.groupDictionary.TryGetValue(recipeType, out var group) &&
            !group.requiredIngredients.ContainsKey(droppedIng.ingredients))
        {
            return false;
        }

        if (playerGameObject != null)
        {
            Pickup pickup = playerGameObject.GetComponent<Pickup>();
            if (pickup != null && pickup.inHandItem == droppedIng.gameObject)
            {
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

        droppedIng.transform.SetParent(this.transform, true);

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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, dropPointRange);

        if (droppedIngredientSpots != null)
        {
            Gizmos.color = Color.green;
            foreach (Transform spot in droppedIngredientSpots)
            {
                if (spot != null)
                    Gizmos.DrawWireSphere(spot.position, 0.15f);
            }
        }

        Gizmos.color = new Color(0.2f, 0.5f, 1f, 0.3f);
        Gizmos.DrawSphere(transform.position, interactionRange);

        Gizmos.color = new Color(0.2f, 0.5f, 1f, 1f);
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}