using UnityEngine;

[CreateAssetMenu(fileName = "New_Plant", menuName = "Plant")]
public class PlantData : ScriptableObject
{
    public Sprite icon;
    public int coinsAmount;
    public float productionInterval;
    [TextArea]
    public string description;
    public bool needToHighlightNearbyCells;

    public enum Rarity
    {
        Common,
        Uncommon,
        Rare
    }  
    public Rarity rarity;
}
