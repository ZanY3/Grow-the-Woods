using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PackManager : ItemPickerBase
{
    public static PackManager Instance;

    [Header("Random Drop")]
    [SerializeField] private List<PlantData> allPlants;
    [SerializeField] private float commonChance = 0.65f;
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
        fliesManager.canLaunchFlies = false;
        coinFallManager.canLaunchEvent = false;
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
        ChangePlaceClueTxt("Click on the <color=yellow>cell</color> where you want to place the <color=green>plant</color>", true);
        fliesManager.canLaunchFlies = true;
        coinFallManager.canLaunchEvent = true;
    }

    public void ChangePlaceClueTxt(string text, bool state)
    {
        placeClueTxt.text = text;
        placeClueTxt.gameObject.SetActive(state);
        if (state) InteractionManager.Instance.canZoomCam = true;
    }

    //Rarity logic stays here, only PackManager needs it
    private PlantData.Rarity GetRandomRarity()
    {
        float roll = Random.value;
        if (roll < rareChance) return PlantData.Rarity.Rare;
        if (roll < rareChance + uncommonChance) return PlantData.Rarity.Uncommon;
        return PlantData.Rarity.Common;
    }

    private PlantData GetRandomPlant(PlantData.Rarity rarity, List<PlantData> excluded)
    {
        var pool = allPlants.FindAll(p => p.rarity == rarity && !excluded.Contains(p));
        return pool.Count == 0 ? null : pool[Random.Range(0, pool.Count)];
    }

    private List<PlantData> GeneratePack()
    {
        var pack = new List<PlantData>();
        for (int i = 0; i < plantsPerPack; i++)
        {
            var rarity = GetRandomRarity();
            var plant = GetRandomPlant(rarity, pack) ?? allPlants[Random.Range(0, allPlants.Count)];
            pack.Add(plant);
        }
        return pack;
    }
}
