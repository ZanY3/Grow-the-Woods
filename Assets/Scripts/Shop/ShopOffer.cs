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
    private int startPrice;

    private Vector3 defaultScale;
    private Vector3 shimmerDefaultScale;
    private Vector3 defaultPosition;

    private Color defaultPriceColor;

    public int Price => offerPrice;
    public int StartPrice => startPrice;

    private void Start()
    {
        startPrice = offerPrice;
        defaultScale = iconObject.transform.localScale;
        defaultPosition = iconObject.transform.localPosition;

        if (shimmer != null)
            shimmerDefaultScale = shimmer.localScale;

        defaultPriceColor = priceTxt.color;

        priceTxt.text = offerPrice.ToString();
    }

    public void UpdatePrice(int newPrice)
    {
        offerPrice = newPrice;
        priceTxt.text = offerPrice.ToString();
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

        iconObject.transform.localScale = defaultScale * selectedScale;
        iconObject.transform.localPosition = defaultPosition;

        if (shimmer != null)
            shimmer.localScale = shimmerDefaultScale * selectedScale;

        shopManager.selectedOfferType = offerType;
        shopManager.selectedOffer = this;

        shopManager.ChangeBuyBtnVisibility(true);
    }
}
