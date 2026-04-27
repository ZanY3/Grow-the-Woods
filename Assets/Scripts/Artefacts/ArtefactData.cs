using UnityEngine;

[CreateAssetMenu(fileName = "New_Artefact", menuName = "Artefact")]
public class ArtefactData : ScriptableObject
{
    public Sprite icon;
    [TextArea]
    public string description;
    public enum Type
    {
        CoinMultiplier,   // Умножает всё золото
        ShopDiscount,     // Скидка на покупку
        ChanceUpgrader    // Шансы выпадения норм предметов увеличиваются
    }
    /*
    public enum UniqueEffect
    {
        None,
        DoubleDrop,
        FreeReroll,
    }
    public UniqueEffect uniqueEffect; // only matters when type == Unique
    */
    public Type type;

}
