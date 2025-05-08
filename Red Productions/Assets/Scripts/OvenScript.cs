using System.Collections.Generic;
using UnityEngine;

public class OvenScript : MonoBehaviour
{
    public float overlapRadius = 10f;
    public LayerMask IngredientLayer;
    public GameObject burgerPrefab;
    public Transform spawnPoint;

    private List<Ingredient.IngredientType> ingredientsInOven = new List<Ingredient.IngredientType>();
    private bool burgerSpawned = false; // Track if a burger has been spawned

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
                        Debug.Log("Ingredient added to oven: " + ingredient.type); // Log when an ingredient is added
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
        GameObject burger = Instantiate(burgerPrefab, spawnPoint.position, Quaternion.identity);
        Debug.Log("Burger Created!");

        // Destroy the ingredients after the burger is spawned
        Collider[] colliders = Physics.OverlapSphere(transform.position, overlapRadius, IngredientLayer);
        foreach (var collider in colliders)
        {
            Destroy(collider.gameObject);
            Debug.Log("Ingredient Destroyed");
        }
        ingredientsInOven.Clear();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, overlapRadius);
    }
}