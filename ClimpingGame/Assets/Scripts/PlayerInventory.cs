using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int rocks = 5;
    public int relics = 0;

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
}