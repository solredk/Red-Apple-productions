using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public enum IngredientType
    {
        burger,
        fries,
        chickenNuggets,
        milkShakes,
    }

    public enum Ingredients
    {
        tomato,
        cheese,
        bread,
        meat,
    }

    public IngredientType type;

}
