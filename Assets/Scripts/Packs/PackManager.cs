using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PackManager : MonoBehaviour
{
    public static PackManager Instance;

    public GameObject plantPrefab;

    [Header("Random drop")]
    [SerializeField] private List<PlantData> allPlants;
    [SerializeField] private float commonChance = 0.65f;
    [SerializeField] private float uncommonChance = 0.25f;
    [SerializeField] private float rareChance = 0.10f;

    [SerializeField] private int plantsPerPack = 3;

    [Header("Pack UI slots (drag here)")]
    [SerializeField] private PlantVisualizer[] packSlots;

    [Space]

    [SerializeField] private GameObject allPanel;
    [SerializeField] private GameObject openPanel;
    [SerializeField] private GameObject plantChoosePanel;
    [SerializeField] private Button confirmBtn;
    [SerializeField] private TMP_Text placeClueTxt;

    [HideInInspector] public bool waitingForClick = false;

    public PlantData selectedPlant;
    private GameObject selectedPlantObject;

    private Dictionary<GameObject, Vector3> originalScales = new Dictionary<GameObject, Vector3>();

    private const float selectedMultiplier = 1.15f;

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

        FillPackUI();

        CachePlantScales();
        AnimatePlants();
    }

    public bool IsExistEmptyCell()
    {
        return Grid.Instance.IsExistEmptyCell();
    }

    public void SelectPlant(GameObject plant)
    {
        if (selectedPlantObject == plant)
        {
            ResetScale(plant);

            selectedPlantObject = null;
            selectedPlant = null;
            confirmBtn.interactable = false;

            return;
        }

        if (selectedPlantObject != null)
        {
            ResetScale(selectedPlantObject);
        }

        selectedPlantObject = plant;
        selectedPlant = plant.GetComponent<PlantVisualizer>().Data;

        SetScale(plant);

        confirmBtn.interactable = true;
    }

    public void ConfirmChoice()
    {
        if (selectedPlantObject != null)
        {
            ResetScale(selectedPlantObject);
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

        if (state)
            InteractionManager.Instance.canZoomCam = true;
    }

    private void CachePlantScales()
    {
        originalScales.Clear();

        foreach (var slot in packSlots)
        {
            RectTransform rect = slot.GetComponent<RectTransform>();
            originalScales.Add(slot.gameObject, rect.localScale);
        }
    }

    private void SetScale(GameObject obj)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();
        Vector3 original = originalScales[obj];

        rect.localScale = new Vector3(
            original.x * selectedMultiplier,
            original.y * selectedMultiplier,
            1f
        );
    }

    private void ResetScale(GameObject obj)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();
        Vector3 original = originalScales[obj];

        rect.localScale = new Vector3(
            original.x,
            original.y,
            1f
        );
    }

    private PlantData.Rarity GetRandomRarity()
    {
        float roll = Random.value;

        if (roll < rareChance)
            return PlantData.Rarity.Rare;

        if (roll < rareChance + uncommonChance)
            return PlantData.Rarity.Uncommon;

        return PlantData.Rarity.Common;
    }

    private PlantData GetRandomPlant(PlantData.Rarity rarity, List<PlantData> excluded)
    {
        List<PlantData> pool = allPlants.FindAll(p => p.rarity == rarity && !excluded.Contains(p));

        if (pool.Count == 0)
            return null;

        return pool[Random.Range(0, pool.Count)];
    }

    private List<PlantData> GeneratePack()
    {
        List<PlantData> pack = new List<PlantData>();

        for (int i = 0; i < plantsPerPack; i++)
        {
            PlantData.Rarity rarity = GetRandomRarity();
            PlantData plant = GetRandomPlant(rarity, pack);

            if (plant == null)
                plant = allPlants[Random.Range(0, allPlants.Count)];

            pack.Add(plant);
        }

        return pack;
    }

    private void FillPackUI()
    {
        List<PlantData> pack = GeneratePack();

        for (int i = 0; i < packSlots.Length; i++)
        {
            if (i >= pack.Count)
            {
                packSlots[i].gameObject.SetActive(false);
                continue;
            }

            packSlots[i].gameObject.SetActive(true);

            packSlots[i].SetData(pack[i]);
            packSlots[i].GetComponent<PlantVisualizer>().VisualizePlant();
        }
    }

    private void AnimatePlants()
    {
        for (int i = 0; i < packSlots.Length; i++)
        {
            RectTransform card = packSlots[i].GetComponent<RectTransform>();

            Vector3 originalScale = originalScales[card.gameObject];
            Vector2 targetPos = card.anchoredPosition;

            card.localScale = new Vector3(0f, 0f, 1f);
            card.anchoredPosition = targetPos + Vector2.up * 20f;

            DG.Tweening.Sequence seq = DOTween.Sequence();

            seq.Append(card.DOScale(originalScale, 0.35f).SetEase(Ease.OutBack));
            seq.Join(card.DOAnchorPos(targetPos, 0.35f).SetEase(Ease.OutCubic));

            seq.SetDelay(i * 0.05f);
        }
    }
}
