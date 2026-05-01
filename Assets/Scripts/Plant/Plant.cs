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
    //[SerializeField] private AudioClip earnSound;
    [Header("Income Text")]
    [SerializeField] private TMP_Text incomeText;

    [Header("Bounce Settings")]
    [SerializeField] private bool enableBounce = true;
    [SerializeField] private float bounceScaleMultiplier = 1.08f;
    [SerializeField] private float bounceDuration = 0.15f;

    [Header("Other")]
    public GameObject fliesAlert;

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

    private void Update()
    {
        if (!InteractionManager.Instance.plantsCanEarn)
        {
            timer = 0f;
            return;
        }

        timer += Time.deltaTime;

        while (timer >= plantData.productionInterval)
        {
            int coins = Mathf.RoundToInt(plantData.coinsAmount * StatsManager.Instance.coinMultiplier);
            coins += StatsManager.Instance.coinAdder;

            if (plantData.name == "Cactus")
            {
                if (!Grid.Instance.HasAdjacentPlants(GetComponentInParent<Cell>()))
                    coins += 3;
            }

            if (plantData.name == "Royal Flower")
            {
                coins += Grid.Instance.CountAdjacentPlants(GetComponentInParent<Cell>());
            }
            if(plantData.name == "Lucky Cap")
            {
                if (Random.value <= 0.25f)
                {
                    coins += 4;
                }
            }

            //AudioManager.Instance.PlaySfxSound(earnSound, 0.025f, 0.8f, 0.95f);
            CoinManager.Instance.AddCoins(coins);

            if (coins > 0)
            {
                ShowIncomeText(coins);

                if (enableBounce && !isBouncing)
                    StartCoroutine(PlantBounce());
            }

            timer -= plantData.productionInterval;
        }
    }

    private void ShowIncomeText(int amount)
    {
        if (incomeText == null) return;

        incomeText.text = "+ " + amount + " <color=#FFD700>C</color>";

        if (incomeRoutine != null)
            StopCoroutine(incomeRoutine);

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
            foreach (var c in highlightedCells)
                c.ResetHighlight();

            highlightedCells.Clear();
            return;
        }

        var neighbors = Grid.Instance.GetAdjacentCells(myCell);

        foreach (var c in neighbors)
        {
            if (!c.isBuyied) continue;

            c.SetHighlight(new Color(0.6f, 1f, 0.6f, 1f));
            highlightedCells.Add(c);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySfxSound(tooltipSound, 0.035f, 0.9f, 1.1f);
        if (!PackManager.Instance.waitingForClick)
            tooltip.SetActive(true);

        if (plantData.needToHighlightNearbyCells)
            HighlightNeighbors(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!PackManager.Instance.waitingForClick)
            tooltip.SetActive(false);

        if (plantData.needToHighlightNearbyCells)
            HighlightNeighbors(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (ShovelSlot.Instance.waitingForAction)
        {
            EndingManager.Instance.UpdateProgress(-1);
            AudioManager.Instance.PlaySfxSound(shovelUsedSound, 0.35f);
            GetComponentInParent<Cell>().isOccupied = false;
            Destroy(gameObject);
            ShovelSlot.Instance.ReturnShowel();
        }
    }
}
