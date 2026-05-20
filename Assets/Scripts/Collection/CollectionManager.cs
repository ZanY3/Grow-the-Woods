using UnityEngine;
using System.Collections.Generic;

public class CollectionManager : MonoBehaviour
{
    public static CollectionManager Instance { get; private set; }

    [SerializeField] private List<PlantData> allPlants;
    [SerializeField] private List<int> unlockedPlantsID = new List<int>();

    private const string SaveKey = "UnlockedPlantsData";

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

    public void UnlockPlant(int id)
    {
        if (id <= 0 || unlockedPlantsID.Contains(id)) return;

        unlockedPlantsID.Add(id);
        SaveData();
    }

    public bool IsPlantUnlocked(int id)
    {
        return unlockedPlantsID.Contains(id);
    }

    public List<PlantData> GetAllPlants()
    {
        return allPlants;
    }

//--------------Save&Load---------------------------------------------------

    public void SaveData()
    {
        string joinedIDs = string.Join(",", unlockedPlantsID);
        PlayerPrefs.SetString(SaveKey, joinedIDs);
        PlayerPrefs.Save();
        Debug.Log($"[SaveSystem] Прогресс сохранен(id растений): {joinedIDs}");
    }

    public void LoadData()
    {
        unlockedPlantsID.Clear();

        if (PlayerPrefs.HasKey(SaveKey))
        {
            string savedString = PlayerPrefs.GetString(SaveKey);
            if (string.IsNullOrEmpty(savedString)) return;

            string[] splitIDs = savedString.Split(',');
            foreach (string idString in splitIDs)
            {
                if (int.TryParse(idString, out int id))
                {
                    unlockedPlantsID.Add(id);
                }
            }
            Debug.Log($"[SaveSystem] - '{SaveKey}' успешно загружен! Открыто растений: {unlockedPlantsID.Count}");
        }
        else
        {
            Debug.Log("[SaveSystem] Файл сохранения не найден. Начинаем новую игру.");
        }
    }

    [ContextMenu("Reset Progress")]
    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        unlockedPlantsID.Clear();
        Debug.Log("[SaveSystem] Прогресс полностью сброшен!");
    }
}