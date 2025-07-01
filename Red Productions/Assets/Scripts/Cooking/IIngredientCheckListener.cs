using UnityEngine;


public interface IIngredientCheckListener
{
 
    void OnIngredientsReady();
    void OnIngredientsMissing(); 
}