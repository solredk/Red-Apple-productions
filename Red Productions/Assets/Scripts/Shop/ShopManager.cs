using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public List<UpgradeEntry> entryupgradeItem;

    [Header("Coins")]
    public int coinsAmount = 0;

    private void Start()
    {
        UpgradeItem upgrade = null;

        foreach (UpgradeEntry entry in entryupgradeItem)
        {
            if (entry.key == "MaxHealthUp")
            {
                upgrade = entry.value;
                entry.value.cost = 100;
                entry.value.level = 1;
                entry.value.maxLevel = 5;
                entry.value.amount = 100;
                break;
            }
        }
    }

    public void MaxHealthUp()
    {
        UpgradeItem upgrade = null;

        foreach (UpgradeEntry entry in entryupgradeItem)
        {
            if (entry.key == "MaxHealthUp")
            {
                upgrade = entry.value;
                break;
            }
        }

        switch (upgrade.level)  
        {
            case 1:
                upgrade.cost = 100;

                upgrade.amount = 130;

                upgrade.level++;
                break;
            case 2:
                upgrade.cost = 200;

                upgrade.amount = 160;

                upgrade.level++;
                break;
            case 3:
                upgrade.cost = 300;

                upgrade.amount = 180;

                upgrade.level++;
                break;
            case 4:
                upgrade.cost = 400;

                upgrade.amount = 200;

                upgrade.level++;
                break;
            default:
                Debug.Log("Max Health Upgrade is at maximum level.");
                return;
        }
    }
    public void AddCoins(int amount)
    {
        coinsAmount += amount;
    }
}
