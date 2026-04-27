using UnityEngine;

[CreateAssetMenu(fileName = "New_Artefact", menuName = "Scriptable Objects/Artefact")]
public class ArtefactData : ScriptableObject
{
    public Sprite icon;
    [TextArea]
    public string description;

    public enum Type
    {
        CoinMultiplier,      // Умножает всё золото
        UpgradeDiscount,     // Скидка на покупку
        PrestigeBonus,       // Бонус к валюте перерождения
        ChanceToDouble       // Шансы выпадения норм предметов увеличиваются
    }
    public Type type;
}
