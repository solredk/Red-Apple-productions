using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using System;


public class IngredientManager : MonoBehaviour
{

    [System.Serializable]
    public class IngredientGroup
    {

        public Ingredient.IngredientType groupType;
        public List<Ingredient.Ingredients> requiredIngredients = new List<Ingredient.Ingredients>();
        public GameObject[] ingredientPrefabs;


    }
    [SerializeField] private List<IngredientGroup> ingredientGroups = new List<IngredientGroup>();
    [SerializeField] private float ingredientCheckRadius = 2f;
    [SerializeField] private LayerMask ingredientLayer;

    private Dictionary<Ingredient.IngredientType, IngredientGroup> groupDictionary = new Dictionary<Ingredient.IngredientType, IngredientGroup>();
    private void Awake()
    {
        foreach (IngredientGroup group in ingredientGroups)
        {
            groupDictionary[group.groupType] = group;
        }

    }

    public List<GameObject> InstantiateIngredientGroup(Ingredient.IngredientType type, Vector3 position, Quaternion rotation)
    {
        if (!groupDictionary.TryGetValue(type, out IngredientGroup group))
        {
            Debug.LogError($"No ingredient group found for type: {type}");
            return null;
        }

        List<GameObject> spawnedIngredients = new List<GameObject>();

        // Check if prefabs array length matches required ingredients count
        if (group.ingredientPrefabs.Length != group.requiredIngredients.Count)
        {
            Debug.LogError($"Mismatch between prefab count and required ingredients for {type}");
            return null;
        }

        for (int i = 0; i < group.ingredientPrefabs.Length; i++)
        {
            if (group.ingredientPrefabs[i] == null)
            {
                GameObject spawnedObj = Instantiate(group.ingredientPrefabs[i], position, rotation);
                spawnedIngredients.Add(spawnedObj);

                position += new Vector3(0.5f, 0, 0);

            }
        }
        return spawnedIngredients;
    }


    public bool CheckIngredientsInRadius(Ingredient.IngredientType type, Vector3 center)
    {
        if (!groupDictionary.TryGetValue(type, out IngredientGroup group))
        {
            Debug.LogError(" No group found for type");

            return false;
        }

        Collider[] colliders = Physics.OverlapSphere(center, ingredientCheckRadius, ingredientLayer);
        List<Ingredient.Ingredients> foundIngredients = new List<Ingredient.Ingredients>();
        foreach (Collider collider in colliders)
        {
            Ingredient ingredient = collider.GetComponent<Ingredient>();
            if (ingredient != null)
            {
                foreach (Ingredient.Ingredients ing in group.requiredIngredients)
                {
                    if (ingredient.ingredients == ing)
                    {
                        foundIngredients.Add(ingredient.ingredients);

                    }
                }
            }
        }
        int foundCount = foundIngredients.Count;
        return foundCount >= group.requiredIngredients.Count;

    }

    public void startIngredientCheck(Ingredient.IngredientType type, Vector3 center, System.Action onComplete)
    {
        StartCoroutine(IngredientCheckCoroutine(type, center, onComplete));
    }

    IEnumerator IngredientCheckCoroutine(Ingredient.IngredientType type, Vector3 center, System.Action onComplete)
    {
        float checkInterval = 0.5f;
        float timeout = 30f;
        float elapsed = 0f;
        while (elapsed < timeout)
        {
            if (CheckIngredientsInRadius(type, center))
            {
                onComplete?.Invoke();
                yield break;

            }
            elapsed += checkInterval;
            yield return new WaitForSeconds(checkInterval);
        }

    }



}