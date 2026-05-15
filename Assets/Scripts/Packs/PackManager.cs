using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PackManager : ItemPickerBase
{
    public static PackManager Instance;
    [SerializeField] private PrestigeManager prestigeManager;

    [Header("Random Drop")]
    [SerializeField] private List<PlantData> region1Plants;
    [SerializeField] private List<PlantData> region2Plants;
    [SerializeField] private float uncommonChance = 0.25f;
    [SerializeField] private float rareChance = 0.10f;
    [SerializeField] private int plantsPerPack = 3;

    [Header("Pack Slots")]
    [SerializeField] private PlantVisualizer[] packSlots;
    [SerializeField] private TMP_Text placeClueTxt;
    [SerializeField] private FliesManager fliesManager;
    [SerializeField] private CoinFallManager coinFallManager;

    [HideInInspector] public bool waitingForClick = false;
    [HideInInspector] public PlantData selectedPlant;
    public GameObject plantPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        cardRects = new RectTransform[packSlots.Length];
        for (int i = 0; i < packSlots.Length; i++)
            cardRects[i] = packSlots[i].GetComponent<RectTransform>();
    }

    public void OpenPack()
    {
        InteractionManager.Instance.canStartEvents = false;
        selectedPlant = null;
        OpenPicker();
    }

    protected override void FillCards()
    {
        var pack = GeneratePack();
        for (int i = 0; i < packSlots.Length; i++)
        {
            bool active = i < pack.Count;
            packSlots[i].gameObject.SetActive(active);
            if (active)
            {
                packSlots[i].SetData(pack[i]);
                packSlots[i].VisualizePlant();
            }
        }
    }

    protected override void OnSelected(int index)
    {
        selectedPlant = packSlots[index].Data;
    }

    protected override void OnDeselected()
    {
        selectedPlant = null;
    }

    protected override void OnConfirmed(int index)
    {
        EndingManager.Instance.ChangeProgressState(false);
        waitingForClick = true;
        AudioManager.Instance.canPlaySounds = true;
        InteractionManager.Instance.canZoomCam = true;
        InteractionManager.Instance.canPressBtns = false;
        InteractionManager.Instance.canStartEvents = true;
        ChangePlaceClueTxt("Click on the <color=yellow>cell</color> where you want to place the <color=green>plant</color>", true);
    }

    public void ChangePlaceClueTxt(string text, bool state)
    {
        placeClueTxt.text = text;
        placeClueTxt.gameObject.SetActive(state);
        if (state) InteractionManager.Instance.canZoomCam = true;
    }

    private List<PlantData> GetCurrentPlants()
    {
        int region = prestigeManager.currentRegion;
        return region switch
        {
            0 => region1Plants,
            1 => region2Plants,
            _ => region2Plants
        };
    }

    private PlantData.Rarity GetRandomRarity()
    {
        float roll = Random.value;
        if (roll < rareChance) return PlantData.Rarity.Rare;
        if (roll < rareChance + uncommonChance) return PlantData.Rarity.Uncommon;
        return PlantData.Rarity.Common;
    }

    private PlantData GetRandomPlant(PlantData.Rarity rarity, List<PlantData> pool, List<PlantData> excluded)
    {
        var filtered = pool.FindAll(p => p.rarity == rarity && !excluded.Contains(p));
        return filtered.Count == 0 ? null : filtered[Random.Range(0, filtered.Count)];
    }

    private List<PlantData> GeneratePack()
    {
        var currentPool = GetCurrentPlants();
        var pack = new List<PlantData>();

        for (int i = 0; i < plantsPerPack; i++)
        {
            var rarity = GetRandomRarity();
            var plant = GetRandomPlant(rarity, currentPool, pack)
                        ?? currentPool[Random.Range(0, currentPool.Count)];
            pack.Add(plant);
        }

        return pack;
    }
}
