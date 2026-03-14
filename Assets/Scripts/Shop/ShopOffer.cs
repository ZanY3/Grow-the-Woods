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
    [SerializeField] private ShopManager shopManager;

    private float selectedScale = 1.1f;
    private Vector3 defaultScale;

    public int Price => offerPrice;

    private void Start()
    {
        defaultScale = iconObject.transform.localScale;
    }

    public void ResetScale()
    {
        iconObject.transform.localScale = defaultScale;
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
        shopManager.selectedOfferType = offerType;
        shopManager.selectedOffer = this;
        shopManager.ChangeBuyBtnVisibility(true);
    }
}
