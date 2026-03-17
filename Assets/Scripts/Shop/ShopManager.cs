using DG.Tweening;
using System;
using System.Diagnostics.Tracing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private ShopOffer[] offers;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject defaultPackPanel;
    [SerializeField] private GameObject buyBtn;

    [Header("Warning")]
    [SerializeField] private GameObject warningObject;

    [Space]
    [Range(1, 2)][SerializeField] private float priceMultiplier;

    [HideInInspector] public ShopOffer.OfferType selectedOfferType;
    [HideInInspector] public ShopOffer selectedOffer;

    private RectTransform shopRect;
    private CanvasGroup shopCanvas;

    private CanvasGroup warningGroup;

    private void Awake()
    {
        shopRect = shopPanel.GetComponent<RectTransform>();
        shopCanvas = shopPanel.GetComponent<CanvasGroup>();

        if (shopCanvas == null)
            shopCanvas = shopPanel.AddComponent<CanvasGroup>();

        warningGroup = warningObject.GetComponent<CanvasGroup>();
        if (warningGroup == null)
            warningGroup = warningObject.AddComponent<CanvasGroup>();
    }

    private void Start()
    {
        ChangeBuyBtnVisibility(false);

        shopPanel.SetActive(false);
        shopCanvas.alpha = 0;

        warningObject.SetActive(false);
        warningGroup.alpha = 0;
    }

    public void ChangeShopVisibility()
    {
        if (!shopPanel.activeSelf)
        {
            OpenShop();
        }
        else
        {
            CloseShop();
        }
    }

    private void OpenShop()
    {
        shopPanel.SetActive(true);

        InteractionManager.Instance.canZoomCam = false;

        shopRect.anchoredPosition = new Vector2(0, -800);
        shopRect.localScale = Vector3.one * 0.9f;
        shopCanvas.alpha = 0;

        shopRect.DOAnchorPosY(0, 0.5f).SetEase(Ease.OutBack);
        shopRect.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
        shopCanvas.DOFade(1, 0.3f);
    }

    private void CloseShop()
    {
        InteractionManager.Instance.canZoomCam = true;

        shopRect.DOAnchorPosY(-800, 0.4f).SetEase(Ease.InBack);
        shopRect.DOScale(0.9f, 0.3f);

        shopCanvas.DOFade(0, 0.25f).OnComplete(() =>
        {
            shopPanel.SetActive(false);

            for (int i = 0; i < offers.Length; i++)
            {
                offers[i].ResetScale();
            }

            selectedOffer = null;
            ChangeBuyBtnVisibility(false);
        });
    }

    public void ChangeBuyBtnVisibility(bool state)
    {
        buyBtn.GetComponent<Button>().interactable = state;
    }

    public void BuyOffer()
    {
        if (!PackManager.Instance.IsExistEmptyCell())
        {
            PlayNoSpaceFeedback();
            return;
        }

        if (selectedOfferType == ShopOffer.OfferType.DefaultPack)
        {
            if (CoinManager.Instance.Coins >= selectedOffer.Price)
            {
                CoinManager.Instance.SpendCoins(selectedOffer.Price);

                CloseShop();
                ChangeBuyBtnVisibility(false);

                InteractionManager.Instance.canZoomCam = false;
                defaultPackPanel.SetActive(true);

                for (int i = 0; i < offers.Length; i++)
                {
                    int newPrice = Mathf.RoundToInt(offers[i].Price * priceMultiplier);
                    offers[i].UpdatePrice(newPrice);
                }
            }
            else
            {
                selectedOffer.PlayNotEnoughMoneyFeedback();
            }
        }
    }

    private void PlayNoSpaceFeedback()
    {
        if (selectedOffer != null)
        {
            selectedOffer.transform.DOKill();

            selectedOffer.transform.DOShakeScale(
                0.3f,
                new Vector3(0.15f, 0.15f, 0),
                15,
                90,
                false
            );
        }

        warningObject.SetActive(true);
        warningGroup.alpha = 0;

        warningGroup.DOFade(1, 0.2f);

        Sequence seq = DOTween.Sequence();

        seq.AppendInterval(1.2f);
        seq.Append(warningGroup.DOFade(0, 0.3f));

        seq.OnComplete(() =>
        {
            warningObject.SetActive(false);
        });
    }
}
