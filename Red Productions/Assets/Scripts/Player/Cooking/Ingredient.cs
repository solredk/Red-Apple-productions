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
// Voor later doe ingredientstates met prefabs als state zo is dan is de ingredient = preset prefab, laat alleen die preset prefab werken en voeg auto feedback ( cooking / uncooked) 