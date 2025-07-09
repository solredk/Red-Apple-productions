using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IngredientManager : MonoBehaviour
{
    [System.Serializable]
    public class IngredientRequirement
    {
        [SerializeField] public Ingredient.Ingredients ingredientType;
        [SerializeField] public int requiredCount = 1;
        [SerializeField] public GameObject presetIngredientPrefab;
    }

    [System.Serializable]
    public class IngredientGroup
    {
        [SerializeField] public Ingredient.IngredientType groupType;
        [SerializeField] public List<IngredientRequirement> ingredientRequirements = new List<IngredientRequirement>();
        [SerializeField] public GameObject[] foodPrefabs;


        public Dictionary<Ingredient.Ingredients, int> requiredIngredients = new Dictionary<Ingredient.Ingredients, int>();

        public void InitializeDictionary()
        {
            requiredIngredients.Clear();
            foreach (IngredientRequirement requirement in ingredientRequirements)
            {
                requiredIngredients[requirement.ingredientType] = requirement.requiredCount;
            }
        }
    }

    [SerializeField] public List<IngredientGroup> ingredientGroups = new List<IngredientGroup>();
    [SerializeField] private float ingredientCheckRadius = 2f;
    [SerializeField] private LayerMask ingredientLayer;

    public Dictionary<Ingredient.IngredientType, IngredientGroup> groupDictionary = new Dictionary<Ingredient.IngredientType, IngredientGroup>();

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

        // complete recipe
        GameObject spawnedObj = Instantiate(group.foodPrefabs[0], position, rotation);

        // Make sure it has a Food component to be recognized by delivery 
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
        List<Ingredient.Ingredients> foundIngredients = new List<Ingredient.Ingredients>();

        foreach (Collider collider in colliders)
        {
            Ingredient ingredient = collider.GetComponent<Ingredient>();
            if (ingredient != null)
            {
                foreach (Ingredient.Ingredients ing in group.requiredIngredients.Keys)
                {
                    if (ingredient.ingredients == ing)
                    {
                        foundIngredients.Add(ing);
                        Debug.Log($"Found ingredient: {ing} for recipe {type}");
                    }
                }
            }
        }

        int foundCount = foundIngredients.Count;
        bool hasAllIngredients = foundCount >= group.requiredIngredients.Count;
        Debug.Log($"Recipe {type}: Found {foundCount}/{group.requiredIngredients.Count} required ingredients");

        return hasAllIngredients;
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
        float timeout = 1.5f; 
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

        // If it here the timeout continued without finding all ingredients 
        listener?.OnIngredientsMissing();
    }
}   