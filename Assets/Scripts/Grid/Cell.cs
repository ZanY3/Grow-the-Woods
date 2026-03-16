using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Cell : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public bool isOccupied = false;

    [SerializeField] private int price;
    [SerializeField] private GameObject closedClue;

    [SerializeField] private TMP_Text priceTxt;

    [Header("Spend coins UI")]
    [SerializeField] private CanvasGroup spendCoinsGroup;
    [SerializeField] private TMP_Text spendCoinsTxt;

    [SerializeField] private Sprite closedCellSprite;
    [SerializeField] private Sprite openedCellSprite;

    [SerializeField] private Color notEnoughColor = new Color(1f, 0.3f, 0.3f); // ДОБАВЛЕНО

    private Color startColor;
    private Color startPriceColor;
    private Vector3 startScale;
    private Image image;

    [Space]
    [Header("Only for first cell with plant")]
    [SerializeField] private GameObject currentPlant;

    public bool isBuyied = false;

    private Tween scaleTween;

    private void Start()
    {
        priceTxt.text = price.ToString();
        spendCoinsTxt.text = "-" + price.ToString();

        spendCoinsGroup.alpha = 0f;
        spendCoinsGroup.gameObject.SetActive(false);

        image = GetComponent<Image>();
        startColor = image.color;
        startPriceColor = priceTxt.color;
        startScale = transform.localScale;

        if (!isBuyied)
        {
            ChangeState(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isBuyied && !ShovelSlot.Instance.waitingForAction && !PackManager.Instance.waitingForClick)
        {
            transform.DOKill();

            scaleTween = transform
                .DOScale(startScale * 1.1f, 0.35f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        if (PackManager.Instance.waitingForClick && !isOccupied && isBuyied)
        {
            image.color = Color.white;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isBuyied)
        {
            transform.DOKill();
            transform.DOScale(startScale, 0.15f);
        }

        if (PackManager.Instance.waitingForClick && isBuyied)
        {
            image.color = startColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (PackManager.Instance.waitingForClick && isBuyied)
        {
            image.color = startColor;

            if (!isOccupied)
            {
                Plant(PackManager.Instance.plantPrefab, PackManager.Instance.selectedPlant);
                InteractionManager.Instance.canPressBtns = true;
            }
            else
            {
                PackManager.Instance.ChangePlaceClueTxt(
                    "This <color=yellow>cell</color> is already occupied!", true);
            }
        }

        if (!isBuyied && !ShovelSlot.Instance.waitingForAction)
        {
            if (CoinManager.Instance.Coins >= price)
            {
                transform.DOKill();
                transform.DOScale(startScale, 0.15f);

                CoinManager.Instance.SpendCoins(price);

                PlaySpendCoinsAnim();

                ChangeState(true);
                isOccupied = false;
            }
            else
            {
                PlayCannotBuyAnim();
            }
        }
    }

    void PlaySpendCoinsAnim()
    {
        spendCoinsGroup.gameObject.SetActive(true);
        spendCoinsGroup.alpha = 1f;

        spendCoinsGroup
            .DOFade(0f, 0.8f)
            .OnComplete(() =>
            {
                spendCoinsGroup.gameObject.SetActive(false);
            });
    }

    void PlayCannotBuyAnim()
    {
        transform.DOKill();
        priceTxt.DOKill();

        priceTxt.color = startPriceColor;

        Sequence seq = DOTween.Sequence();

        seq.Append(priceTxt.DOColor(notEnoughColor, 0.15f));

        seq.Join(
            transform.DOShakeScale(
                0.35f,
                new Vector3(0.08f, 0.08f, 0f),
                15,
                90,
                false
            )
        );

        seq.Append(priceTxt.DOColor(startColor, 0.25f));
    }

    public void ChangeState(bool state)
    {
        if (state)
        {
            closedClue.SetActive(false);
            image.sprite = openedCellSprite;
            image.color = startColor;
            transform.localScale = Vector3.one;
        }
        else
        {
            image.sprite = closedCellSprite;
            image.color = new Color(0.5f, 0.5f, 0.5f, 0.75f);
            closedClue.SetActive(true);
        }

        isBuyied = state;
    }

    public void Plant(GameObject plantPrefab, PlantData plantData)
    {
        if (isOccupied) return;

        currentPlant = Instantiate(plantPrefab, transform, false);
        currentPlant.GetComponent<PlantVisualizer>().SetData(plantData);
        currentPlant.GetComponent<PlantVisualizer>().VisualizePlant();
        RectTransform rect = currentPlant.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
        rect.localRotation = Quaternion.identity;

        PackManager.Instance.ChangePlaceClueTxt("", false);

        isOccupied = true;
        PackManager.Instance.waitingForClick = false;
    }
}