using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    public PlayerInventory inventory;

    [Header("UI")]
    public TMP_Text rocksText;
    public TMP_Text relicsText;

    private void Update()
    {
        rocksText.text = "Rocks: " + inventory.rocks;
        relicsText.text = "Relics: " + inventory.relics;
    }
}