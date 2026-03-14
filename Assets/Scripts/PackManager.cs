using UnityEngine;
using UnityEngine.UI;

public class PackManager : MonoBehaviour
{
    [SerializeField] private GameObject allPanel;
    [SerializeField] private GameObject openPanel;
    [SerializeField] private GameObject plantChoosePanel;
    [SerializeField] private Button confirmBtn;

    private PlantData selectedPlant;
    private GameObject selectedPlantObject;

    private float selectedScale = 1.1f;
    private const float baseScale = 1.5f;

    public void OpenPack()
    {
        openPanel.SetActive(false);
        plantChoosePanel.SetActive(true);

        confirmBtn.interactable = false;
        selectedPlant = null;
        selectedPlantObject = null;
    }

    public void SelectPlant(GameObject plant)
    {
        if (selectedPlantObject == plant)
        {
            SetScale(plant, baseScale);

            selectedPlantObject = null;
            selectedPlant = null;
            confirmBtn.interactable = false;

            return;
        }

        if (selectedPlantObject != null)
        {
            SetScale(selectedPlantObject, baseScale);
        }

        selectedPlantObject = plant;
        selectedPlant = plant.GetComponent<PlantVisualizer>().Data;

        SetScale(plant, baseScale * selectedScale);

        confirmBtn.interactable = true;
    }

    public void ConfirmChoice()
    {
        openPanel.SetActive(false);
        plantChoosePanel.SetActive(false);
        allPanel.SetActive(false);
    }

    private void SetScale(GameObject obj, float scale)
    {
        Vector3 currentScale = obj.transform.localScale;
        obj.transform.localScale = new Vector3(scale, scale, currentScale.z);
    }
}
