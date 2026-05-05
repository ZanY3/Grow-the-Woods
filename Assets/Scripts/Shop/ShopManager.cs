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
    [SerializeField] private GameObject plantPackPanel;
    [SerializeField] private GameObject artefactPackPanel;
    [SerializeField] private GameObject buyBtn;

    [SerializeField] private AudioClip buySound;
    [SerializeField] private AudioClip shopBtnSound;

    [Header("Warning")]
    [SerializeField] private GameObject warningObject;
    [SerializeField] private AudioClip warningSound;

    [Space]
    [Range(1, 2)][SerializeField] private float priceMultiplier;
    [Space]
    [SerializeField] private ArtefactsManager artefactsManager;

    [HideInInspector] public ShopOffer.OfferType selectedOfferType;
    public ShopOffer selectedOffer;

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
        AudioManager.Instance.PlaySfxSound(shopBtnSound, 0.35f);
        if (!shopPanel.activeSelf)
            OpenShop();
        else
            CloseShop();
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
                offers[i].ResetScale();

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
        if ((selectedOfferType == ShopOffer.OfferType.PlantPack && !Grid.Instance.IsExistEmptyCell()) ||
            (selectedOfferType == ShopOffer.OfferType.ArtefactPack && !artefactsManager.ExistEmptySlots()))
        {
            PlayNoSpaceFeedback();
            return;
        }

        if (CoinManager.Instance.Coins < selectedOffer.Price)
        {
            selectedOffer.PlayNotEnoughMoneyFeedback();
            return;
        }

        if (selectedOfferType == ShopOffer.OfferType.PlantPack ||
            selectedOfferType == ShopOffer.OfferType.ArtefactPack)
        {
            AudioManager.Instance.PlaySfxSound(buySound, 0.5f, 0.9f, 1.1f);
            CoinManager.Instance.SpendCoins(selectedOffer.Price);

            CloseShop();
            ChangeBuyBtnVisibility(false);
            InteractionManager.Instance.canZoomCam = false;

            if (selectedOfferType == ShopOffer.OfferType.PlantPack)
                plantPackPanel.SetActive(true);
            else
                artefactPackPanel.SetActive(true);

            // Compound the base price — discount is applied separately inside ShopOffer.Price
            selectedOffer.CompoundPrice();
        }
    }

    // Call this from ArtefactsManager after equipping or removing a discount artefact
    public void RefreshAllPrices()
    {
        for (int i = 0; i < offers.Length; i++)
            offers[i].RefreshPriceDisplay();
    }

    private void PlayNoSpaceFeedback()
    {
        AudioManager.Instance.PlaySfxSound(warningSound, 0.5f);

        if (selectedOffer != null)
        {
            Transform t = selectedOffer.transform;

            t.DOKill();
            t.localScale = Vector3.one;

            t.DOShakeScale(
                0.3f,
                new Vector3(0.15f, 0.15f, 0),
                15,
                90,
                false
            ).OnComplete(() =>
            {
                t.localScale = Vector3.one;
            });
        }

        warningGroup.DOKill();
        warningObject.SetActive(true);
        warningGroup.alpha = 0;

        Sequence seq = DOTween.Sequence();
        seq.Append(warningGroup.DOFade(1, 0.2f));
        seq.AppendInterval(1.2f);
        seq.Append(warningGroup.DOFade(0, 0.3f));
        seq.OnComplete(() => warningObject.SetActive(false));
    }

    public void ReturnPrices()
    {
        for (int i = 0; i < offers.Length; i++)
            offers[i].ResetToStartPrice();
    }
}