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

        // Check if prefabs array length matches required ingredients count
        if (group.foodPrefabs.Length != group.requiredIngredients.Count)
        {
            Debug.LogError($"Mismatch between prefab count and required ingredients for {type}");
            return null;
        }

        for (int i = 0; i < group.foodPrefabs.Length; i++)
        {
            if (group.foodPrefabs[i] != null)
            {
                GameObject spawnedObj = Instantiate(group.foodPrefabs[i], position, rotation);
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
        
        //  ingredients found
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