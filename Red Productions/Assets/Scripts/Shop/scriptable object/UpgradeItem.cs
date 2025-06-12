using UnityEngine;
[CreateAssetMenu(fileName = "UpgradeItem", menuName = "Scriptable Objects/UpgradeItem")]
public class UpgradeItem : ScriptableObject
{
    public string itemName;
    public string description;
    public int cost;
    public float amount;
    public int level;
    public int maxLevel;

}
