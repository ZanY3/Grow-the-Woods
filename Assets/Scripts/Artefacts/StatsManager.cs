using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance { get; private set; }

    [Header("Stats")]
    [HideInInspector] public float coinMultiplier = 1;
    [HideInInspector] public float shopDiscount = 0;  //Like +67%
    [HideInInspector] public int chanceAdder = 0;     //Like +40%

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
