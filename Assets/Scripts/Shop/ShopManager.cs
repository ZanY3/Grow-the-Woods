using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private ShopOffer[] offers;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject defaultPackPanel;
    [SerializeField] private GameObject buyBtn;

    [HideInInspector] public ShopOffer.OfferType selectedOfferType;
    [HideInInspector] public ShopOffer selectedOffer;


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
            InteractionManager.Instance.canZoomCam = true;
        }
        else
        {
            InteractionManager.Instance.canZoomCam = false;
        }
    }

    public void ChangeBuyBtnVisibility(bool state)
    {
        if(PackManager.Instance.IsExistEmptyCell())
        {
            buyBtn.GetComponent<Button>().interactable = state;
        }
    }
    public void BuyOffer()
    {
        if(selectedOfferType == ShopOffer.OfferType.DefaultPack)
        {
            if(CoinManager.Instance.Coins >= selectedOffer.Price)
            {
                Debug.Log(selectedOfferType.ToString());
                CoinManager.Instance.SpendCoins(selectedOffer.Price);

                ChangeShopVisibility();
                ChangeBuyBtnVisibility(false);
                InteractionManager.Instance.canZoomCam = false;
                defaultPackPanel.SetActive(true);
            }
            else
            {
                Debug.LogError("Not enough money to buy this!");
            }
        }
    }
}
