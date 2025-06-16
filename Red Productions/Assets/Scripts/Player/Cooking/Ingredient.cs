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
        // burger ingredients
        tomato,
        cheese,
        bread,
        meat,

        // ingredients
        cup,
        fruitbasket,

        // fries
        fries,
        friesbag,
        // nuggets 
        nuggets,
        nuggetbag,
    }
    public Ingredients ingredients;

    public IngredientType type;

}
