using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private ShopOffer[] offers;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject buyBtn;

    [HideInInspector] public ShopOffer.OfferType offerType;
    [HideInInspector]public ShopOffer selectedOffer;


    private void Start()
    {
        ChangeBuyBtnVisibility(false);
    }

    public void ChangeShopVisibility()
    {
        bool newState = !shopPanel.activeSelf;
        shopPanel.SetActive(newState);

        if (!newState)
        {
            for (int i = 0; i < offers.Length; i++)
            {
                offers[i].ResetScale();
            }

            selectedOffer = null;
            ChangeBuyBtnVisibility(false);
        }
    }

    public void ChangeBuyBtnVisibility(bool state)
    {
        buyBtn.GetComponent<Button>().interactable = state;
    }
}
