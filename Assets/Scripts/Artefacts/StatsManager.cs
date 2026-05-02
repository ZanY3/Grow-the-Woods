using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance { get; private set; }

    [Header("Stats")]
    public float coinMultiplier = 1;
    public int coinAdder = 0;       //+some coins to income
    public int chanceAdder = 0;     //Like +40%

    [Range(0, 1)] public float shopDiscount = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void ApplyArtefact(ArtefactData artefact)
    {
        ModifyStats(artefact, 1);
    }

    public void RemoveArtefact(ArtefactData artefact)
    {
        ModifyStats(artefact, -1);
    }

    private void ModifyStats(ArtefactData artefact, int sign)
    {
        switch (artefact.type)
        {
            case ArtefactData.Type.CoinMultiplier:
                coinMultiplier += sign * artefact.value;
                break;
            case ArtefactData.Type.CoinAdder:
                coinAdder += sign * (int)artefact.value;
                break;
            case ArtefactData.Type.ShopDiscount:
                // FIX: Clamp so stacking multiple discount artefacts never reaches 100% (free)
                shopDiscount = Mathf.Clamp(shopDiscount + sign * artefact.value, 0f, 0.9f);
                break;
            case ArtefactData.Type.ChanceUpgrader:
                chanceAdder += sign * (int)artefact.value;
                break;
        }
    }
}