using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private ShopManager shopManager;

    [SerializeField] private UpgradeItem upgradeItem;

    [SerializeField] private TextMeshProUGUI buttonText;

    [SerializeField] private Button button;

    private void Start()
    {
        UpdateButtonUI();
        upgradeItem.cost = 100; 
        upgradeItem.level = 1; 
        upgradeItem.maxLevel = 5; 
        upgradeItem.amount = 100; 
    }

    public void UpgradeMaxHealth()
    {
        shopManager.MaxHealthUp();
        UpdateButtonUI();
    }

    private void UpdateButtonUI()
    {
        if (shopManager.coinsAmount >= upgradeItem.cost)
        {
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
        }
        buttonText.text = $"Upgrade {upgradeItem.name} Cost: {upgradeItem.cost} Level: {upgradeItem.level}/{upgradeItem.maxLevel}";
    }
}
