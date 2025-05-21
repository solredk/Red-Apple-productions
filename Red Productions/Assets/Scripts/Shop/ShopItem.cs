using UnityEngine;

[CreateAssetMenu(fileName = "ShopItem", menuName = "Scriptable Objects/ShopItem")]
public class ShopItem : ScriptableObject
{
    [SerializeField] private string Itemname;
    [SerializeField] private string description;

    [SerializeField] private int price;
    [SerializeField] private int amount;
}
