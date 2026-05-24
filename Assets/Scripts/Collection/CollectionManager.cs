using UnityEngine;
using System.Collections.Generic;

public class CollectionManager : MonoBehaviour
{
    public static CollectionManager Instance { get; private set; }

    [SerializeField] private List<PlantData> allPlants;
    [SerializeField] private List<ArtefactData> allArtefacts;

    [SerializeField] private List<int> unlockedPlantsID = new List<int>();
    [SerializeField] private List<int> unlockedArtefactsID = new List<int>();

    private const string PlantSaveKey = "UnlockedPlantsData";
    private const string ArtefactSaveKey = "UnlockedArtefactsData";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UnlockPlant(1);
    }

    // ── Plants ────────────────────────────────────────────────────────────────

    public void UnlockPlant(int id)
    {
        if (id <= 0 || unlockedPlantsID.Contains(id)) return;
        unlockedPlantsID.Add(id);
        SaveData();
    }

    public bool IsPlantUnlocked(int id) => unlockedPlantsID.Contains(id);
    public List<PlantData> GetAllPlants() => allPlants;

    // ── Artefacts ─────────────────────────────────────────────────────────────

    public void UnlockArtefact(int runtimeId)
    {
        if (runtimeId <= 0 || unlockedArtefactsID.Contains(runtimeId)) return;
        unlockedArtefactsID.Add(runtimeId);
        SaveData();
    }

    public bool IsArtefactUnlocked(int runtimeId) => unlockedArtefactsID.Contains(runtimeId);
    public List<ArtefactData> GetAllArtefacts() => allArtefacts;

    public int GetArtefactRuntimeId(ArtefactData artefact)
    {
        int index = allArtefacts.IndexOf(artefact);
        return index >= 0 ? index + 1 : -1;
    }

    // ── Save & Load ───────────────────────────────────────────────────────────

    public void SaveData()
    {
        PlayerPrefs.SetString(PlantSaveKey, string.Join(",", unlockedPlantsID));
        PlayerPrefs.SetString(ArtefactSaveKey, string.Join(",", unlockedArtefactsID));
        PlayerPrefs.Save();
        Debug.Log($"[SaveSystem] Plants saved: {string.Join(",", unlockedPlantsID)}");
        Debug.Log($"[SaveSystem] Artefacts saved: {string.Join(",", unlockedArtefactsID)}");
    }

    public void LoadData()
    {
        unlockedPlantsID.Clear();
        unlockedArtefactsID.Clear();
        LoadIDs(PlantSaveKey, unlockedPlantsID, "plants");
        LoadIDs(ArtefactSaveKey, unlockedArtefactsID, "artefacts");
    }

    private void LoadIDs(string key, List<int> target, string label)
    {
        if (!PlayerPrefs.HasKey(key))
        {
            Debug.Log($"[SaveSystem] No save for {label}, starting fresh.");
            return;
        }
        string saved = PlayerPrefs.GetString(key);
        if (string.IsNullOrEmpty(saved)) return;
        foreach (string s in saved.Split(','))
            if (int.TryParse(s, out int id))
                target.Add(id);
        Debug.Log($"[SaveSystem] Loaded {target.Count} {label}.");
    }

    [ContextMenu("Reset Progress")]
    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey(PlantSaveKey);
        PlayerPrefs.DeleteKey(ArtefactSaveKey);
        unlockedPlantsID.Clear();
        unlockedArtefactsID.Clear();
        Debug.Log("[SaveSystem] Progress reset.");
    }
}