using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvenScript : MonoBehaviour
{
    [Header("Oven Settings")]
    [SerializeField] private GameObject burgerPrefab;
    [SerializeField] private Transform spawnPoint;
    
    private float pickupDelay = 0.1f;

    private float overlapRadius = 10f;
    private bool burgerSpawned = false; 
    
    private GameObject spawnedBurger; 
    private LayerMask IngredientLayer;
    private List<Ingredient.IngredientType> ingredientsInOven = new List<Ingredient.IngredientType>();

    void Start()
    {
        ingredientsInOven = new List<Ingredient.IngredientType>();
    }

    void Update()
    {
        if (!burgerSpawned)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, overlapRadius, IngredientLayer);

            ingredientsInOven.Clear();

            foreach (var collider in colliders)
            {
                Ingredient ingredient = collider.GetComponent<Ingredient>();
                if (ingredient != null)
                {
                    if (!ingredientsInOven.Contains(ingredient.type))
                    {
                        ingredientsInOven.Add(ingredient.type);
                        Debug.Log("Ingredient added to oven: " + ingredient.type); 
                    }
                }
            }

            if (ingredientsInOven.Count == 3)
            {
                SpawnBurger();
                burgerSpawned = true;
            }
        }
    }

    void SpawnBurger()
    {
        spawnedBurger = Instantiate(burgerPrefab, spawnPoint.position, Quaternion.identity);
        Debug.Log("Burger Created!");

        // Start the coroutine to delay pickup
        StartCoroutine(EnablePickupAfterDelay(spawnedBurger));

        Collider[] colliders = Physics.OverlapSphere(transform.position, overlapRadius, IngredientLayer);
        foreach (var collider in colliders)
        {
            Destroy(collider.gameObject);
            Debug.Log("Ingredient Destroyed");
        }
        ingredientsInOven.Clear();
    }

    IEnumerator EnablePickupAfterDelay(GameObject burger)
    {
        // Disable the burger's collider initially
        Collider burgerCollider = burger.GetComponent<Collider>();
        if (burgerCollider != null)
            burgerCollider.enabled = false;

        // Wait for the specified delay
        yield return new WaitForSeconds(pickupDelay);

        // Enable the collider after the delay
        if (burgerCollider != null)
            burgerCollider.enabled = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, overlapRadius);
    }
}