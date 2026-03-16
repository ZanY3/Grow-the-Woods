using TMPro;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    [SerializeField] private TMP_Text coinsTxt;
    [HideInInspector] public static CoinManager Instance;
    private int coins = 10;
    public int Coins => coins;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        coinsTxt.text = coins.ToString();
    }
    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateUI();
    }
    public void SpendCoins(int amount)
    {
        coins -= amount;
        UpdateUI();
    }
}
