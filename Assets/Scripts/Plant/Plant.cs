using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Plant : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [HideInInspector] public PlantData plantData;

    [SerializeField] private GameObject tooltip;
    [SerializeField] private AudioClip tooltipSound;
    [SerializeField] private AudioClip shovelUsedSound;
    [Header("Income Text")]
    [SerializeField] private TMP_Text incomeText;

    [Header("Bounce Settings")]
    [SerializeField] private bool enableBounce = true;
    [SerializeField] private float bounceScaleMultiplier = 1.08f;
    [SerializeField] private float bounceDuration = 0.15f;

    [Header("Other")]
    public GameObject fliesAlert;
    [HideInInspector] public bool podConnected = false;
    [HideInInspector] public Plant currentPartner;

    private static HashSet<Plant> pairedPlants = new HashSet<Plant>();

    private float timer;
    private Vector3 originalScale;
    private bool isBouncing;
    private Coroutine incomeRoutine;
    private List<Cell> highlightedCells = new List<Cell>();

    private void Start()
    {
        originalScale = transform.localScale;

        if (incomeText != null)
            incomeText.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (plantData.name == "Twin Pod")
        {
            BreakConnection();
        }
        pairedPlants.Remove(this);
    }

    private void Update()
    {
        if (!InteractionManager.Instance.plantsCanEarn)
        {
            timer = 0f;
            return;
        }

        // --- ЛОГИКА GROW SHROOM (Ускорение) ---
        float speedMultiplier = 1f;

        // Обычные растения проверяют, нет ли рядом Grow Shroom
        if (plantData.name != "Grow Shroom")
        {
            speedMultiplier = GetGrowthSpeedMultiplier();
        }

        // Таймер идет быстрее, если speedMultiplier > 1
        timer += Time.deltaTime * speedMultiplier;

        while (timer >= plantData.productionInterval)
        {
            ExecuteProduction();
            timer -= plantData.productionInterval;
        }
    }

    private float GetGrowthSpeedMultiplier()
    {
        float multiplier = 1f;
        Cell myCell = GetComponentInParent<Cell>();
        if (myCell == null) return multiplier;

        List<Cell> adjacentCells = Grid.Instance.GetAdjacentCells(myCell);

        foreach (var cell in adjacentCells)
        {
            if (cell.isOccupied)
            {
                Plant neighbor = cell.GetComponentInChildren<Plant>();
                // Если сосед — Grow Shroom, даем +50% к скорости (0.5f)
                if (neighbor != null && neighbor.plantData.name == "Grow Shroom")
                {
                    multiplier += 0.5f;
                }
            }
        }
        return multiplier;
    }

    private void ExecuteProduction()
    {
        int coins = Mathf.RoundToInt(plantData.coinsAmount * StatsManager.Instance.coinMultiplier);
        coins += StatsManager.Instance.coinAdder;
        coins += GetPlantBonus(coins);

        CoinManager.Instance.AddCoins(coins);

        if (coins > 0)
        {
            ShowIncomeText(coins);

            if (enableBounce && !isBouncing)
                StartCoroutine(PlantBounce());
        }
    }

    private int GetPlantBonus(int currentCoins)
    {
        int bonus = 0;
        Cell myCell = GetComponentInParent<Cell>();
        if (myCell == null) return 0;

        if (plantData.name == "Cactus")
        {
            if (!Grid.Instance.HasAdjacentPlants(myCell))
                bonus += 3;
        }
        else if (plantData.name == "Royal Flower")
        {
            bonus += Grid.Instance.CountAdjacentPlants(myCell);
        }
        else if (plantData.name == "Lucky Cap")
        {
            if (Random.value <= 0.25f)
                bonus += 4;
        }
        else if (plantData.name == "Bamboo")
        {
            var columnCells = Grid.Instance.GetColumnCells(myCell);
            foreach (var c in columnCells)
            {
                if (c.isBuyied && !c.isOccupied)
                    bonus++;
            }
        }
        else if (plantData.name == "Twin Pod")
        {
            if (!pairedPlants.Contains(this))
            {
                podConnected = false;
                List<Cell> adjacentCells = Grid.Instance.GetAdjacentCells(myCell);

                for (int i = 0; i < adjacentCells.Count; i++)
                {
                    if (!adjacentCells[i].isOccupied) continue;
                    Plant neighborPlant = adjacentCells[i].GetComponentInChildren<Plant>();

                    if (neighborPlant != null && neighborPlant.plantData.name == "Twin Pod" && !pairedPlants.Contains(neighborPlant))
                    {
                        pairedPlants.Add(this);
                        pairedPlants.Add(neighborPlant);
                        this.podConnected = true;
                        neighborPlant.podConnected = true;
                        this.currentPartner = neighborPlant;
                        neighborPlant.currentPartner = this;
                        break;
                    }
                }
            }

            if (podConnected)
                bonus += currentCoins;
        }

        // Grow Shroom сам по себе может не давать бонусов, 
        // так как его главная фишка в Update() выше.

        return bonus;
    }

    // --- ОСТАЛЬНЫЕ МЕТОДЫ (BreakConnection, UI, Highlight и т.д.) БЕЗ ИЗМЕНЕНИЙ ---

    public void BreakConnection()
    {
        if (currentPartner != null)
        {
            currentPartner.podConnected = false;
            pairedPlants.Remove(currentPartner);
            currentPartner.currentPartner = null;
        }
        podConnected = false;
        pairedPlants.Remove(this);
        currentPartner = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (ShovelSlot.Instance.waitingForAction)
        {
            if (plantData.name == "Twin Pod") BreakConnection();
            EndingManager.Instance.UpdateProgress(-1);
            AudioManager.Instance.PlaySfxSound(shovelUsedSound, 0.35f);
            GetComponentInParent<Cell>().isOccupied = false;
            Destroy(gameObject);
            ShovelSlot.Instance.ReturnShowel();
        }
    }

    private void ShowIncomeText(int amount)
    {
        if (incomeText == null) return;
        incomeText.text = "+ " + amount + " <color=#FFD700>C</color>";
        if (incomeRoutine != null) StopCoroutine(incomeRoutine);
        incomeRoutine = StartCoroutine(FadeIncomeText());
    }

    private System.Collections.IEnumerator FadeIncomeText()
    {
        float fadeIn = 0.1f;
        float visibleTime = 0.25f;
        float fadeOut = 0.2f;
        Color baseColor = incomeText.color;
        incomeText.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);
        incomeText.gameObject.SetActive(true);
        float t = 0f;
        while (t < fadeIn)
        {
            float a = t / fadeIn;
            incomeText.color = new Color(baseColor.r, baseColor.g, baseColor.b, a);
            t += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(visibleTime);
        t = 0f;
        while (t < fadeOut)
        {
            float a = 1f - (t / fadeOut);
            incomeText.color = new Color(baseColor.r, baseColor.g, baseColor.b, a);
            t += Time.deltaTime;
            yield return null;
        }
        incomeText.gameObject.SetActive(false);
    }

    private System.Collections.IEnumerator PlantBounce()
    {
        isBouncing = true;
        float time = 0f;
        Vector3 targetScale = originalScale * bounceScaleMultiplier;
        while (time < bounceDuration)
        {
            float t = time / bounceDuration;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            time += Time.deltaTime;
            yield return null;
        }
        time = 0f;
        while (time < bounceDuration)
        {
            float t = time / bounceDuration;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localScale = originalScale;
        isBouncing = false;
    }

    private void HighlightNeighbors(bool state)
    {
        if (!plantData.needToHighlightNearbyCells) return;
        Cell myCell = GetComponentInParent<Cell>();
        if (myCell == null) return;
        if (!state)
        {
            foreach (var c in highlightedCells) c.ResetHighlight();
            highlightedCells.Clear();
            return;
        }
        var neighbors = Grid.Instance.GetAdjacentCells(myCell);
        foreach (var c in neighbors)
        {
            if (!c.isBuyied) continue;
            if (!c.isOccupied)
            {
                c.SetHighlight(new Color(0.6f, 1f, 0.6f, 1f));
                highlightedCells.Add(c);
            }
        }
    }

    private void HighlightColumn(bool state)
    {
        if (!plantData.needToHighlightColumn) return;
        Cell myCell = GetComponentInParent<Cell>();
        if (myCell == null) return;
        if (!state)
        {
            foreach (var c in highlightedCells) c.ResetHighlight();
            highlightedCells.Clear();
            return;
        }
        var columnCells = Grid.Instance.GetColumnCells(myCell);
        foreach (var c in columnCells)
        {
            if (c == myCell) continue;
            if (!c.isBuyied) continue;
            if (!c.isOccupied)
            {
                c.SetHighlight(new Color(0.6f, 1f, 0.6f, 1f));
                highlightedCells.Add(c);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySfxSound(tooltipSound, 0.035f, 0.9f, 1.1f);
        if (!PackManager.Instance.waitingForClick) tooltip.SetActive(true);
        if (plantData.needToHighlightNearbyCells) HighlightNeighbors(true);
        if (plantData.needToHighlightColumn) HighlightColumn(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!PackManager.Instance.waitingForClick) tooltip.SetActive(false);
        HighlightNeighbors(false);
        HighlightColumn(false);
    }
}