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

    // Default multiplier (used for region 0, or as fallback if no region override is set)
    [SerializeField] private float priceMultiplier;

    // Optional per-region overrides. Index = region index (0, 1, 2...).
    // If a region has no entry (out of range, or value <= 0), priceMultiplier is used instead.
    [SerializeField] private float[] regionPriceMultipliers;

    [SerializeField] private GameObject unavailableTxtObj;

    [Header("Feedback")]
    [SerializeField] private Color notEnoughMoneyColor = Color.red;
    [SerializeField] private AudioClip errorSound;
    [SerializeField] private AudioClip offerChooseSound;

    private float selectedScale = 1.1f;

    // startPrice never changes — it's the original serialized value
    private int startPrice;

    // currentBasePrice compounds with the active multiplier each purchase, NO discount applied
    // discount is only applied on top when displaying or buying
    private int currentBasePrice;

    // The multiplier currently in effect (set via SetRegionMultiplier, defaults to priceMultiplier)
    private float activeMultiplier;

    private bool isActive = true;

    private Vector3 defaultScale;
    private Vector3 shimmerDefaultScale;
    private Vector3 defaultPosition;
    private Color defaultPriceColor;
    public int StartPrice => startPrice;
    public int CurrentBasePrice => currentBasePrice;

    [HideInInspector] public int purchasedTimes = 0;

    public int Price => Mathf.Max(
        Mathf.RoundToInt(currentBasePrice * (1f - Mathf.Clamp(StatsManager.Instance.shopDiscount, 0f, 0.9f))),
        1
    );

    private void Start()
    {
        startPrice = offerPrice;
        currentBasePrice = offerPrice;
        activeMultiplier = priceMultiplier;

        defaultScale = iconObject.transform.localScale;
        defaultPosition = iconObject.transform.localPosition;

        if (shimmer != null)
            shimmerDefaultScale = shimmer.localScale;

        defaultPriceColor = priceTxt.color;
        RefreshPriceDisplay();
    }

    void Update()
    {
        // Only the artefact pack has a region-0 purchase cap. Recompute both ways every
        // frame so it re-enables automatically once currentRegion advances past 0.
        if (offerType == OfferType.ArtefactPack)
        {
            bool shouldBeUnavailable = purchasedTimes >= 3 && PrestigeManager.Instance.currentRegion == 0;
        }
    }

    // Called by ShopManager when the region changes, so CompoundPrice() uses the right multiplier
    public void SetRegionMultiplier(int region)
    {
        if (regionPriceMultipliers != null && region >= 0 && region < regionPriceMultipliers.Length && regionPriceMultipliers[region] > 0f)
            activeMultiplier = regionPriceMultipliers[region];
        else
            activeMultiplier = priceMultiplier;
    }

    // Called after each purchase to compound the base price
    public void CompoundPrice()
    {
        currentBasePrice = Mathf.RoundToInt(currentBasePrice * activeMultiplier);
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
        if(!isActive) return;
        
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