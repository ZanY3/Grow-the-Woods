using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance { get; private set; }

    [Header("Stats")]
    public float coinMultiplier = 1;
    public int coinAdder = 0;                     //+some coins to income
    public int chanceAdder = 0;                   //Like +40%
    [Range(0, 1)] public float shopDiscount = 0;
    //[HideInInspector]

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
}
