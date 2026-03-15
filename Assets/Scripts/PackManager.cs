using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PackManager : MonoBehaviour
{
    public static PackManager Instance;
    public GameObject plantPrefab;
    [SerializeField] private Cell[] cells;

    [SerializeField] private GameObject allPanel;
    [SerializeField] private GameObject openPanel;
    [SerializeField] private GameObject plantChoosePanel;
    [SerializeField] private Button confirmBtn;
    [SerializeField] private TMP_Text placeClueTxt;

    [HideInInspector] public bool waitingForClick = false;

    private PlantData selectedPlant;
    private GameObject selectedPlantObject;

    private float selectedScale = 1.2f;
    private const float baseScale = 1.5f;
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }
    public void OpenPack()
    {
        openPanel.SetActive(false);
        plantChoosePanel.SetActive(true);

        confirmBtn.interactable = false;
        selectedPlant = null;
        selectedPlantObject = null;
    }
    public bool IsExistEmptyCell()
    {
        for(int i = 0; i <  cells.Length; i++)
        {
            if (!cells[i].isOccupied && cells[i].isBuyied)
                return true;
        }
        return false;
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
        if (selectedPlantObject != null)
        {
            SetScale(selectedPlantObject, baseScale);
        }

        openPanel.SetActive(true);
        plantChoosePanel.SetActive(false);
        allPanel.SetActive(false);

        waitingForClick = true;
        InteractionManager.Instance.canZoomCam = true;
        InteractionManager.Instance.canPressBtns = false;

        ChangePlaceClueTxt(
            "Click on the <color=yellow>cell</color> where you want to place the <color=green>plant</color>",
            true
        );
    }
    public void ChangePlaceClueTxt(string text, bool state)
    {
        placeClueTxt.text = text;
        placeClueTxt.gameObject.SetActive(state);
        if(state)
            InteractionManager.Instance.canZoomCam = true;
    }

    private void SetScale(GameObject obj, float scale)
    {
        Vector3 currentScale = obj.transform.localScale;
        obj.transform.localScale = new Vector3(scale, scale, currentScale.z);
    }
}
