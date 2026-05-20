using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CollectionSlot : MonoBehaviour
{
    [SerializeField] private Image plantIcon;

    private PlantData currentPlantData;
    private bool isUnlocked;

    public void Setup(PlantData data, bool unlockedState)
    {
        currentPlantData = data;
        isUnlocked = unlockedState;

        if (currentPlantData == null || plantIcon == null) return;

        plantIcon.sprite = currentPlantData.icon;

        if (isUnlocked)
        {
            plantIcon.color = Color.white;
        }
        else
        {
            plantIcon.color = Color.black;
        }
    }
}
