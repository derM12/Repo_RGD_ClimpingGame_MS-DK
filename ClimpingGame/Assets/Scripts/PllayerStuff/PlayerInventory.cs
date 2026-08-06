using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int rocks = 5;
    public int relics = 0;

    public GameObject pickaxeObject;

    public bool hasPickaxe = false;

    public bool UseRock()
    {
        if (rocks <= 0)
        {
            Debug.Log("No rocks left!");
            return false;
        }
        rocks--;
        Debug.Log("Rock thrown. Rocks remaining: " + rocks);
        return true;
    }

    public void AddRelic()
    {
        relics++;
        Debug.Log("Relic picked up! Total relics: " + relics);
    }

    public bool TradeRelicForRock()
    {
        if (relics <= 0)
        {
            Debug.Log("No relics to trade!");
            return false;
        }
        relics--;
        rocks++;
        Debug.Log("Traded 1 relic for 1 rock. Relics: " + relics + " | Rocks: " + rocks);
        return true;
    }

    public bool BuyPickaxe()
    {
        if (hasPickaxe)
        {
            Debug.Log("Already own the pickaxe!");
            return false;
        }

        if (relics < 10)
        {
            Debug.Log("Need 10 relics!");
            return false;
        }

        relics -= 10;
        hasPickaxe = true;
        Debug.Log("Bought the Pickaxe!");

        if (pickaxeObject != null)
            pickaxeObject.SetActive(true);
        Debug.Log("Activated the Pickaxe!");

        return true;
    }
}