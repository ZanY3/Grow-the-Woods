using UnityEngine;
using UnityEngine.EventSystems;

public class ShopOffer : MonoBehaviour, IPointerClickHandler
{
    public enum OfferType
    {
        Empty,
        DefaultPack
    };

    [SerializeField] private OfferType offerType;
    [SerializeField] private int offerPrice;

    [SerializeField] private GameObject iconObject;
    [SerializeField] private RectTransform shimmer; // ← shimmer

    [SerializeField] private ShopManager shopManager;

    private float selectedScale = 1.1f;

    private Vector3 defaultScale;
    private Vector3 shimmerDefaultScale;

    public int Price => offerPrice;

    private void Start()
    {
        defaultScale = iconObject.transform.localScale;

        if (shimmer != null)
            shimmerDefaultScale = shimmer.localScale;
    }

    public void ResetScale()
    {
        iconObject.transform.localScale = defaultScale;

        if (shimmer != null)
            shimmer.localScale = shimmerDefaultScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (shopManager.selectedOffer == this)
        {
            ResetScale();

            shopManager.selectedOffer = null;
            shopManager.selectedOfferType = OfferType.Empty;
            shopManager.ChangeBuyBtnVisibility(false);

            return;
        }

        iconObject.transform.localScale = defaultScale * selectedScale;

        if (shimmer != null)
            shimmer.localScale = shimmerDefaultScale * selectedScale;

        shopManager.selectedOfferType = offerType;
        shopManager.selectedOffer = this;

        shopManager.ChangeBuyBtnVisibility(true);
    }
}
