using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlantVisualizer : MonoBehaviour
{
    [SerializeField] private PlantData data;
    [SerializeField] private Image iconImg;

    [Space]
    [Header("Tooltip")]
    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private TMP_Text descriptionTxt;
    [SerializeField] private TMP_Text rarityTxt;

    private Plant plant;

    public PlantData Data => data;

    private void Start()
    {
        plant = GetComponent<Plant>();

        if (data != null)
        {
            SetData(data);
        }

        VisualizePlant();
    }

    public void SetData(PlantData dataToSet)
    {
        data = dataToSet;

        if (plant != null)
            plant.plantData = data;
    }

    public void VisualizePlant()
    {
        iconImg.sprite = data.icon;
        nameTxt.text = data.name;
        descriptionTxt.text = data.description;

        if (data.rarity == PlantData.Rarity.Common)
            rarityTxt.color = Color.green;
        else if (data.rarity == PlantData.Rarity.Uncommon)
            rarityTxt.color = Color.cyan;
        else if (data.rarity == PlantData.Rarity.Rare)
            rarityTxt.color = Color.red;

        rarityTxt.text = data.rarity.ToString();
    }
}
