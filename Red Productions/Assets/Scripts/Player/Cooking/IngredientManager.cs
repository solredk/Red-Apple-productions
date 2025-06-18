using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class IngredientManager : MonoBehaviour
{
    [Serializable]
    public class IngredientRequirement
    {
        [SerializeField] public Ingredient.Ingredients ingredientType;
        [SerializeField] public int requiredCount = 1;
    }

    [Serializable]
    public class IngredientGroup
    {
        [SerializeField] public Ingredient.IngredientType groupType;
        [SerializeField] public List<IngredientRequirement> ingredientRequirements = new List<IngredientRequirement>();
        [SerializeField] public GameObject[] foodPrefabs;

        // Dictionary populated from the serializable list
        public Dictionary<Ingredient.Ingredients, int> requiredIngredients = new Dictionary<Ingredient.Ingredients, int>();

        // Method to initialize dictionary from the list
        public void InitializeDictionary()
        {
            requiredIngredients.Clear();
            foreach (IngredientRequirement requirement in ingredientRequirements)
            {
                requiredIngredients[requirement.ingredientType] = requirement.requiredCount;
            }
        }
    }

    [SerializeField] private List<IngredientGroup> ingredientGroups = new List<IngredientGroup>();
    [SerializeField] private float ingredientCheckRadius = 2f;
    [SerializeField] private LayerMask ingredientLayer;

    private Dictionary<Ingredient.IngredientType, IngredientGroup> groupDictionary = new Dictionary<Ingredient.IngredientType, IngredientGroup>();

    private void Awake()
    {
        foreach (IngredientGroup group in ingredientGroups)
        {
            group.InitializeDictionary();
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

        // Check if we have food prefabs to spawn
        if (group.foodPrefabs == null || group.foodPrefabs.Length == 0)
        {
            Debug.LogError($"No food prefabs defined for {type}");
            return null;
        }

        // For food items, just spawn the first one (the completed recipe)
        GameObject spawnedObj = Instantiate(group.foodPrefabs[0], position, rotation);

        // Make sure it has a Food component to be recognized by delivery points
        if (!spawnedObj.GetComponent<Food>())
        {
            spawnedObj.AddComponent<Food>();
        }

        spawnedIngredients.Add(spawnedObj);
        Debug.Log($"Successfully spawned {type} food item: {spawnedObj.name}");

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

        // Use HashSet instead of List to automatically handle duplicates
        HashSet<Ingredient.Ingredients> foundIngredients = new HashSet<Ingredient.Ingredients>();

        foreach (Collider collider in colliders)
        {
            Ingredient ingredient = collider.GetComponent<Ingredient>();
            if (ingredient != null)
            {
                foreach (KeyValuePair<Ingredient.Ingredients, int> pair in group.requiredIngredients)
                {
                    if (ingredient.ingredients == pair.Key)
                    {
                        foundIngredients.Add(ingredient.ingredients);
                    }
                }
            }
        }

        // Check if we found all the required ingredients
        int foundCount = foundIngredients.Count;
        return foundCount >= group.requiredIngredients.Count;
    }

    public float GetCheckRadius()
    {
        return ingredientCheckRadius;
    }

    public LayerMask GetIngredientLayer()
    {
        return ingredientLayer;
    }

    public void startIngredientCheck(Ingredient.IngredientType type, Vector3 center, IIngredientCheckListener listener)
    {
        StartCoroutine(IngredientCheckCoroutine(type, center, listener));
    }

    IEnumerator IngredientCheckCoroutine(Ingredient.IngredientType type, Vector3 center, IIngredientCheckListener listener)
    {
        float checkInterval = 0.5f;
        float timeout = 30f;
        float elapsed = 0f;
        while (elapsed < timeout)
        {
            if (CheckIngredientsInRadius(type, center))
            {
                listener?.OnIngredientsReady();
                yield break;
            }
            elapsed += checkInterval;
            yield return new WaitForSeconds(checkInterval);
        }
    }
}