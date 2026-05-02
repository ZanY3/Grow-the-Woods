using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopOffer : MonoBehaviour, IPointerClickHandler
{
    public enum OfferType
    {
        Empty,
        PlantPack,
        ArtefactPack
    };

    [SerializeField] private OfferType offerType;
    [SerializeField] private int offerPrice;
    [SerializeField] private GameObject iconObject;
    [SerializeField] private TMP_Text priceTxt;
    [SerializeField] private RectTransform shimmer;
    [SerializeField] private ShopManager shopManager;

    [Header("Feedback")]
    [SerializeField] private Color notEnoughMoneyColor = Color.red;
    [SerializeField] private AudioClip errorSound;
    [SerializeField] private AudioClip offerChooseSound;

    private float selectedScale = 1.1f;

    // startPrice never changes — it's the original serialized value
    private int startPrice;

    // currentBasePrice compounds with priceMultiplier each purchase, NO discount applied
    // discount is only applied on top when displaying or buying
    private int currentBasePrice;

    private Vector3 defaultScale;
    private Vector3 shimmerDefaultScale;
    private Vector3 defaultPosition;
    private Color defaultPriceColor;

    public int StartPrice => startPrice;
    public int CurrentBasePrice => currentBasePrice;

    // Price shown/used for buying = currentBasePrice with discount applied
    public int Price => Mathf.Max(
        Mathf.RoundToInt(currentBasePrice * (1f - Mathf.Clamp(StatsManager.Instance.shopDiscount, 0f, 0.9f))),
        1
    );

    private void Start()
    {
        startPrice = offerPrice;
        currentBasePrice = offerPrice;

        defaultScale = iconObject.transform.localScale;
        defaultPosition = iconObject.transform.localPosition;

        if (shimmer != null)
            shimmerDefaultScale = shimmer.localScale;

        defaultPriceColor = priceTxt.color;
        RefreshPriceDisplay();
    }

    // Called after each purchase to compound the base price
    public void CompoundPrice(float multiplier)
    {
        currentBasePrice = Mathf.RoundToInt(currentBasePrice * multiplier);
        RefreshPriceDisplay();
    }

    // Called when discount artefact is equipped or removed — recalculates display only
    public void RefreshPriceDisplay()
    {
        priceTxt.text = Price.ToString();
    }

    // Called by ReturnPrices() to fully reset
    public void ResetToStartPrice()
    {
        currentBasePrice = startPrice;
        RefreshPriceDisplay();
    }

    // Legacy — kept for any other callers
    public void UpdatePrice(int newPrice)
    {
        currentBasePrice = newPrice;
        RefreshPriceDisplay();
    }

    public void ResetScale()
    {
        iconObject.transform.localScale = defaultScale;
        iconObject.transform.localPosition = defaultPosition;
        if (shimmer != null)
            shimmer.localScale = shimmerDefaultScale;
    }

    Vector3 GetTargetScale()
    {
        if (shopManager.selectedOffer == this)
            return defaultScale * selectedScale;
        return defaultScale;
    }

    public void PlayNotEnoughMoneyFeedback()
    {
        AudioManager.Instance.PlaySfxSound(errorSound, 0.5f);

        Transform icon = iconObject.transform;
        icon.DOKill();
        priceTxt.DOKill();
        icon.localPosition = defaultPosition;
        icon.localScale = GetTargetScale();

        icon.DOShakePosition(
            duration: 0.3f,
            strength: 12f,
            vibrato: 20,
            randomness: 90,
            snapping: false,
            fadeOut: true
        );

        icon.DOPunchScale(
            icon.localScale * 0.15f,
            0.25f,
            10,
            1
        );

        Sequence seq = DOTween.Sequence();
        seq.Append(priceTxt.DOColor(notEnoughMoneyColor, 0.08f));
        seq.Append(priceTxt.DOColor(defaultPriceColor, 0.25f));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySfxSound(offerChooseSound, 0.15f, 0.9f, 1.1f);

        if (shopManager.selectedOffer == this)
        {
            ResetScale();
            shopManager.selectedOffer = null;
            shopManager.selectedOfferType = OfferType.Empty;
            shopManager.ChangeBuyBtnVisibility(false);
            return;
        }

        if (shopManager.selectedOffer != null)
            shopManager.selectedOffer.ResetScale();

        iconObject.transform.localScale = defaultScale * selectedScale;
        iconObject.transform.localPosition = defaultPosition;

        if (shimmer != null)
            shimmer.localScale = shimmerDefaultScale * selectedScale;

        shopManager.selectedOfferType = offerType;
        shopManager.selectedOffer = this;
        shopManager.ChangeBuyBtnVisibility(true);
    }
}